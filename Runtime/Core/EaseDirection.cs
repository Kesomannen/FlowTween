using System;

namespace FlowTween {

[Flags]
public enum EaseDirection {
    In = 1 << 0,
    Out = 1 << 1,
    InOut = In | Out
}

}