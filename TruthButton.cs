
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TruthButton : UdonSharpBehaviour
{
    public TruthOrDareControl game;

    void Start()
    {
        
    }

    public override void Interact()
    {
        game._RequestQuestion(true);
    }
}
