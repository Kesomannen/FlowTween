using System.Collections;

namespace FlowTween { 
    
public static class CoroutineUtil {
    public static IEnumerator Yield(this object obj) {
        yield return obj;
    }
}

}