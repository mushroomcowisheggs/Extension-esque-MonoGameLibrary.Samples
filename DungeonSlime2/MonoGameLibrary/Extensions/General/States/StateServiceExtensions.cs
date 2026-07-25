using System;

namespace MonoGameLibrary.Extensions.General.States {
    public static class StateServiceExtensions {
        public static bool IsEmpty(this IStateService service) {
            return service == null || service.CurrentState == null;
        }
        
        public static Type GetCurrentStateType(this IStateService service) {
            return service?.CurrentState?.GetType();
        }

        public static bool IsInState<T>(this IStateService service) where T : State {
            return service != null && service.CurrentState is T;
        }
    }
}