using System;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;
using System.Threading;

namespace TrainControl
{
    public class MotorController
    {
        private const int motorRight = 20; // GPIO 20
        private const int motorLeft = 21;  // GPIO 21
        private const int pwmChannel = 12; // GPIO 12
        private const int pwmFrequency = 100;
        private const int stopDelayMs = 1000;

        private GpioController gpioController;
        private PwmChannel pwmController;
        private bool disposed = false;

        public MotorController()
        {
            gpioController = new GpioController(PinNumberingScheme.Logical);
            try
            {
                pwmController = new SoftwarePwmChannel(pwmChannel, pwmFrequency, 0);
                gpioController.OpenPin(motorRight, PinMode.Output);
                gpioController.OpenPin(motorLeft, PinMode.Output);
                pwmController.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GPIOピンの初期化でエラーが発生: {ex.Message}");
                throw;
            }
        }

        public async Task GoForward(int durationMs)
        {
            await StopAsync();
            Console.WriteLine("前進");
            pwmController.DutyCycle = 0;
            gpioController.Write(motorRight, PinValue.High);
            gpioController.Write(motorLeft, PinValue.Low);
            await Accelerate();

            await Task.Delay(durationMs);

            await Decelerate();
            await StopAsync();
        }

        public async Task GoBackward(int durationMs)
        {
            await StopAsync();
            Console.WriteLine("後退");
            gpioController.Write(motorRight, PinValue.Low);
            gpioController.Write(motorLeft, PinValue.High);
            await Accelerate();

            await Task.Delay(durationMs);

            await Decelerate();
            await StopAsync();
        }


        public async Task StopAsync()
        {
            try
            {
                Console.WriteLine("停止");
                pwmController.DutyCycle = 0;
                gpioController.Write(motorRight, PinValue.High);
                gpioController.Write(motorLeft, PinValue.High);
                await Task.Delay(stopDelayMs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"停止処理中にエラーが発生: {ex.Message}");
            }
        }

        private async Task Accelerate()
        {
            for (int i = 0; i <= 40; i++)
            {
                pwmController.DutyCycle = (double)i / 100;
                await Task.Delay(100);
            }
        }

        private async Task Decelerate()
        {
            for (int i = 40; i >= 0; i--)
            {
                pwmController.DutyCycle = (double)i / 100;
                await Task.Delay(100);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if(disposing)
                {
                    pwmController?.Stop();
                    gpioController?.ClosePin(motorRight);
                    gpioController?.ClosePin(motorLeft);
                    gpioController?.Dispose();
                    pwmController?.Dispose();
                }
                disposed = true;
            }
        }
    }
}
