using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CodeBlock : MonoBehaviour
{
    public virtual bool CanAcceptBlock(CodeBlock block) { return false; }
    public virtual void AcceptBlock(CodeBlock block) { }
    public abstract void Execute();
}

