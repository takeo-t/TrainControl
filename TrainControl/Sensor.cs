using Iot.Device.Adc;
using System;
using System.Device.Spi;
using System.Threading;

namespace AdcTutorial
{
    public class Sensor : IDisposable
    {
        private bool disposed = false;
        private readonly SpiDevice spi;
        private readonly Mcp3008 mcp;

        public Sensor()
        {
            // SPI接続の設定
            var hardwareSpiSettings = new SpiConnectionSettings(0, 0);
            spi = SpiDevice.Create(hardwareSpiSettings);
            mcp = new Mcp3008(spi);
        }

        // センサーから値を読み取るメソッド
        public async Task ReadSensorData()
        {
            while (true)
            {
                Console.Clear();
                double valueCh0 = mcp.Read(0);
                Console.WriteLine($"Channel 0: {valueCh0}");
                Console.WriteLine($"Channel 0: {Math.Round(valueCh0 / 10.23, 1)}%");

                double valueCh1 = mcp.Read(1);
                Console.WriteLine($"Channel 1: {valueCh1}");
                Console.WriteLine($"Channel 1: {Math.Round(valueCh1 / 10.23, 1)}%");

                await Task.Delay(500);
            }
        }

        // リソースの解放
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    mcp?.Dispose();
                    spi?.Dispose();
                }
                disposed = true;
            }
        }

        // IDisposableの実装
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal double GetSensorValue()
        {
            // センサーのチャンネル0の値を取得
            double sensorValue = mcp.Read(0);
            return sensorValue;
        }

    }
}
