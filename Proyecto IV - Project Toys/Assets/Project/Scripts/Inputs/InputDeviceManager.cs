using UnityEngine;
using UnityEngine.InputSystem;

public static class InputDeviceManager
{
   
   public enum Devices
   {
      Mando,
      Teclado
   }
   
   public static Devices currentDevice { get; private set; }
   
   public static string DispositivoActual { get; private set; } = "Ninguno";
   
   public static event System.Action<Devices> AlCambiarDispositivo;
   
   [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
   private static void Inicializar()
   {
      InputSystem.onActionChange += DetectarCambioDispositivo;
      Debug.Log("Gestor de Dispositivos Estático Inicializado.");
   }

   private static void DetectarCambioDispositivo(object obj, InputActionChange change)
   {
      if (change == InputActionChange.ActionPerformed)
      {
         InputAction accion = (InputAction)obj;
         InputDevice dispositivo = accion.activeControl.device;

         if (dispositivo is Gamepad && DispositivoActual != "Mando")
         {
            DispositivoActual = "Mando";
            currentDevice = Devices.Mando;
            AlCambiarDispositivo?.Invoke(currentDevice);
         }
         
         else if ((dispositivo is Keyboard || dispositivo is Mouse) && DispositivoActual != "Teclado")
         {
            DispositivoActual = "Teclado";
            currentDevice = Devices.Teclado;
            AlCambiarDispositivo?.Invoke(currentDevice);
         }
      }
   }
   
   
}
