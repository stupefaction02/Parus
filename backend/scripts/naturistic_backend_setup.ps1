Start-Process cmd -ArgumentList "/k `"c: & cd C:\Users\Ivan\Desktop\Sensorium\.NET\ASP.NET\Projects\NatureForYou\backend & title Backend & echo dotnet run --project Naturistic.Backend --launch-profile Naturistic.Backend`""
Start-Process cmd -ArgumentList "/k `"c: & cd C:\Users\Ivan\Desktop\Sensorium\.NET\ASP.NET\Projects\NatureForYou\backend\Naturistic.Hsl\bin\Debug\netcoreapp3.1 & title HSL service & echo Naturistic.Hsl.exe`""
Start-Process cmd -ArgumentList "/k `"c: & cd C:\Users\Ivan\Desktop\Sensorium\.NET\ASP.NET\Projects\NatureForYou\backend\DummyPublisher\bin\Debug\net5.0 & title DummyPublisher & echo DummyPublisher.exe`""