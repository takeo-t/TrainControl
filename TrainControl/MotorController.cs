using System.Device.Gpio;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;

namespace TrainControl
{
    public class MotorController : IDisposable
    {
        /// <summary>
        /// モーター制御用のGPIOピン番号 GPIO20
        /// </summary>
        private const int motorRight = 20;
        /// <summary>
        /// モーター制御用のGPIOピン番号 GPIO21
        /// </summary>
        private const int motorLeft = 21;
        /// <summary>
        /// モーター制御用のPWMチャンネル番号 GPIO12
        /// </summary>
        private const int pwmChannel = 12;
        /// <summary>
        /// pwmの周波数
        /// </summary>
        private const int pwmFrequency = 100;
        /// <summary>
        /// 停止時の待機時間
        /// </summary>
        private const int stopDelayMs = 1000;

        private GpioController gpioController;
        private PwmChannel pwmController;
        private bool disposed = false;

        public double MaximumDutyCycle { get; set; } = 0.4;

        /// <summary>
        /// 初期化
        /// </summary>
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
        /// <summary>
        /// 前進
        /// </summary>
        /// <param name="durationMs"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 後退
        /// </summary>
        /// <param name="durationMs"></param>
        /// <returns></returns>
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

        /// <summary>
        /// モーターを停止する
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// モーターを加速させる
        /// </summary>
        /// <returns></returns>
        private async Task Accelerate()
        {
            int steps = (int)(MaximumDutyCycle * 100);

            for (int i = 0; i <= steps; i++)
            {
                pwmController.DutyCycle = (double)i / steps * MaximumDutyCycle;
                await Task.Delay(100);
            }
        }

        /// <summary>
        /// モーターを減速させる
        /// </summary>
        /// <returns></returns>
        private async Task Decelerate()
        {
            int steps = (int)(MaximumDutyCycle * 100);

            for (int i = steps; i >= 0; i--)
            {
                pwmController.DutyCycle = (double)i / steps * MaximumDutyCycle;
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
