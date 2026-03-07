using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public static class InputDeviceManager
{
   
   public enum Devices
   {
      MandoGenerico,
      Teclado,
      MandoXbox,
      MandoPlayStation
   }
   
   public static Devices CurrentDevice { get; private set; }
   
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

         switch (dispositivo)
         {
            case Keyboard:
               if (DispositivoActual != "Teclado")
               {
                  UpdateDevice("Teclado", Devices.Teclado);
               }
               break;
            case Mouse:
               if (DispositivoActual != "Teclado")
               {
                  UpdateDevice("Teclado", Devices.Teclado);
               }
               break;
            /*case Gamepad:
               if (dispositivo is DualShockGamepad && DispositivoActual != "MandoPlayStation")
               {
                  UpdateDevice("MandoPlayStation", Devices.MandoPlayStation);
               }
               else if (dispositivo is XInputController && DispositivoActual != "MandoXbox")
               {
                  UpdateDevice("MandoXbox", Devices.MandoXbox);
               }
               break;*/
            case DualShockGamepad:
               if (DispositivoActual != "MandoPlayStation")
                  UpdateDevice("MandoPlayStation", Devices.MandoPlayStation);
               break;

            case XInputController:
               if (DispositivoActual != "MandoXbox")
                  UpdateDevice("MandoXbox", Devices.MandoXbox);
               break;
            case Gamepad:
               if (DispositivoActual != "MandoGenerico")
                  UpdateDevice("MandoGenerico", Devices.MandoGenerico);
               break;
               
               
         }

         /*if (dispositivo is Gamepad && DispositivoActual != "Mando")
         {
            DispositivoActual = "Mando";
            currentDevice = Devices.Mando;
            AlCambiarDispositivo?.Invoke(currentDevice);
         }
         
         else if ((dispositivo is Keyboard || dispositivo is Mouse) && DispositivoActual != "Teclado")
         {
            UpdateDevice("Teclado", Devices.Teclado);
         }*/
      }
   }

   private static void UpdateDevice(string name, Devices device)
   {
      DispositivoActual = name;
      CurrentDevice = device;
      AlCambiarDispositivo?.Invoke(CurrentDevice);
   }
   
   
}
