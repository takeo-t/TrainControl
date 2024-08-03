﻿using System;
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

        private GpioController gpioController;
        private PwmChannel pwmController;

        public MotorController()
        {
            gpioController = new GpioController(PinNumberingScheme.Logical);
            try
            {
                pwmController = new SoftwarePwmChannel(pwmChannel, pwmFrequency, 0);
                gpioController.OpenPin(motorRight, PinMode.Output);
                gpioController.OpenPin(motorLeft, PinMode.Output);
                pwmController.Start();
                Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing GPIO pins: {ex.Message}");
                throw;
            }
        }

        public void GoForward(int durationMs)
        {
            Console.WriteLine("前進");
            pwmController.DutyCycle = 0;
            gpioController.Write(motorRight, PinValue.High);
            gpioController.Write(motorLeft, PinValue.Low);
            Accelerate();

            Thread.Sleep(durationMs);

            Decelerate();
            Stop();
        }

        public void GoBackward(int durationMs)
        {
            Console.WriteLine("後退");
            gpioController.Write(motorRight, PinValue.Low);
            gpioController.Write(motorLeft, PinValue.High);
            Accelerate();

            Thread.Sleep(durationMs);

            Decelerate();
            Stop();
        }

        public void Stop()
        {
            Console.WriteLine("停止中");
            pwmController.DutyCycle = 0;
            gpioController.Write(motorRight, PinValue.High);
            gpioController.Write(motorLeft, PinValue.High);
        }

        private void Accelerate()
        {
            for (int i = 0; i <= 40; i++)
            {
                pwmController.DutyCycle = (double)i / 100;
                Thread.Sleep(100);
            }
        }

        private void Decelerate()
        {
            for (int i = 40; i >= 0; i--)
            {
                pwmController.DutyCycle = (double)i / 100;
                Thread.Sleep(100);
            }
        }

        ~MotorController()
        {
            pwmController.Stop();
            gpioController.ClosePin(motorRight);
            gpioController.ClosePin(motorLeft);
            gpioController.Dispose();
            pwmController.Dispose();
        }
    }
}
