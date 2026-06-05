using UnityEngine;
using UnityEngine.AI;

public class InimigoAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform jogador;
    private float alturaFixaDaAgua;

    [Header("Configurações de Combate")]
    [Tooltip("Distância lateral que o barco vai manter do jogador para atirar")]
    public float distanciaDeCombate = 5f; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        alturaFixaDaAgua = transform.position.y;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            jogador = playerObj.transform;
        }

        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (jogador == null) return;

        // 1. Calcula a posição lateral esquerda e direita do jogador
        Vector3 ladoDireitoDoJogador = jogador.position + (jogador.right * distanciaDeCombate);
        Vector3 ladoEsquerdoDoJogador = jogador.position - (jogador.right * distanciaDeCombate);

        // 2. Descobre qual dos dois lados está mais perto do inimigo atualmente
        float distDireita = Vector3.Distance(transform.position, ladoDireitoDoJogador);
        float distEsquerda = Vector3.Distance(transform.position, ladoEsquerdoDoJogador);
        
        Vector3 alvoFinal = (distDireita < distEsquerda) ? ladoDireitoDoJogador : ladoEsquerdoDoJogador;

        // 3. Manda o NavMesh navegar em direção a esse ponto lateral
        agent.SetDestination(alvoFinal);

        // 4. Quando o inimigo estiver chegando perto do ponto lateral, ele força o alinhamento
        float distanciaDoAlvo = Vector3.Distance(transform.position, alvoFinal);
        if (distanciaDoAlvo < 3f) 
        {
            AlinharComOJogador();
        }

        // Mantém o barco nivelado na água
        Vector3 posicaoAtual = transform.position;
        posicaoAtual.y = alturaFixaDaAgua;
        transform.position = posicaoAtual;
    }

    void AlinharComOJogador()
    {
        // Força o barco inimigo a olhar para a mesma direção (frente) que o jogador está olhando.
        // Isso faz com que as laterais de ambos fiquem paralelas!
        Quaternion rotacaoAlvo = Quaternion.LookRotation(jogador.forward);
        
        // Gira suavemente para alinhar
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacaoAlvo, Time.deltaTime * 2f);
    }
}