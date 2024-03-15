using System.Diagnostics;
using Newtonsoft.Json;
using ParallelProgrammingLab1.PetriNetSemaphore;

namespace ParallelProgrammingLab1;

internal static class Program
{
    private static int Main()
    {
        try
        {
            var inputData = JsonConvert.DeserializeObject<InputData>(File.ReadAllText("Input.json"));
            var random = new Random();

            if (inputData == null)
                throw new Exception(
                    "Невозможно прочитать данные из файла конфигурации. Вероятно, данные не соответствуют формату.");

            if (inputData.Qt < 1)
                inputData.Qt = random.Next(1, 1001);
            if (inputData.MaxT < 1)
                inputData.MaxT = random.Next(1, 11);
            if (inputData.MaxP < 1)
                inputData.MaxP = random.Next(1, inputData.Threads.Count + 1);

            Resource.OutputFile = new StreamWriter("Output.txt", false);

            var pa = inputData.Pa ? "SJF, preemptive, absolute priority" : "SJF, nonpreemptive";
            Resource.OutputFile.WriteLine($"Pa: {pa}");
            Resource.OutputFile.WriteLine($"Qt: {inputData.Qt}");
            Resource.OutputFile.WriteLine($"MaxT: {inputData.MaxT}");
            Resource.OutputFile.WriteLine($"MaxP: {inputData.MaxP}");

            Resource.OutputFile.WriteLine("\nРесурсы:");
            foreach (var res in inputData.Resources)
            {
                if (res.Capacity < 1)
                    res.Capacity = random.Next(1, inputData.Threads.Count / 2 + 1);
                    
                Resource.CreateResource(res);
                Resource.OutputFile.WriteLine($"\tНазвание: {res.Name}; пропускная способность: {res.Capacity}");
            }

            Resource.OutputFile.WriteLine("\nПотоки:");
            foreach (var thread in inputData.Threads)
            {
                var resources = new List<Resource>();
                foreach (var resName in thread.ResourceNames)
                {
                    resources.Add(Resource.GetByName(resName));
                }

                if (thread.Priority < 1)
                    thread.Priority = random.Next(1, inputData.MaxP + 1);
                if (thread.CpuBurst < 1)
                    thread.CpuBurst = random.Next(1, inputData.MaxT + 1);
                if (thread.Quantity < 1)
                    thread.Quantity = random.Next(1, 6);

                _ = new MyThread(thread.Name, thread.Priority, thread.CpuBurst, thread.Quantity, resources);
                Resource.OutputFile.WriteLine(
                    $"\tНазвание: {thread.Name}; приоритет: {thread.Priority}; время работы в квантах: {thread.CpuBurst}; сколько раз выполнить: {thread.Quantity}; названия требуемых ресурсов: {string.Join(", ", thread.ResourceNames)}");
            }

            Resource.Timeslice = inputData.Qt;
            Resource.Preemptive = inputData.Pa;

            Resource.OutputFile.WriteLine("\nПоквантовый мониторинг состояния ресурсов и потоков:");

            var stopwatch = new Stopwatch();

            foreach (var thread in MyThread.Threads)
            {
                Task.Run(() => thread.Execute(Resource.Preemptive));
            }

            stopwatch.Start();
            Resource.Execute();
            Resource.OutputFile.WriteLine($"\nОбщее время работы системы: {stopwatch.ElapsedMilliseconds} мс.");

            Resource.OutputFile.Dispose();
            
            Console.Write("Нажмите любую клавишу для завершения программы...");
            Console.ReadKey();

            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return 1;
        }
    }
}
