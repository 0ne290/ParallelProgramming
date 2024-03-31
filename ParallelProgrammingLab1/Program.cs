using System.Diagnostics;
using Newtonsoft.Json;

namespace ParallelProgrammingLab1;

internal static class Program
{
    private static int Main()
    {
        try
        {
            var inputData = JsonConvert.DeserializeObject<InputData>(File.ReadAllText("../../../Input.json"));
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

            var outputFile = new StreamWriter("../../../Output.txt", false);

            var pa = inputData.Pa ? "SJF, preemptive, absolute priority" : "SJF, nonpreemptive";
            outputFile.WriteLine($"Pa: {pa}");
            outputFile.WriteLine($"Qt: {inputData.Qt}");
            outputFile.WriteLine($"MaxT: {inputData.MaxT}");
            outputFile.WriteLine($"MaxP: {inputData.MaxP}");

            outputFile.WriteLine("\nРесурсы:");
            foreach (var res in inputData.Resources)
            {
                if (res.Capacity < 1)
                    res.Capacity = random.Next(1, inputData.Threads.Count / 2 + 1);
                    
                _ = new PetriNet.Semaphore(res.Name, res.Capacity);
                outputFile.WriteLine($"\tНазвание: {res.Name}; пропускная способность: {res.Capacity}");
            }

            outputFile.WriteLine("\nПотоки:");
            foreach (var thread in inputData.Threads)
            {
                var resources = new List<ParallelProgrammingLab1.PetriNet.Semaphore>();
                foreach (var resName in thread.ResourceNames)
                    resources.Add(PetriNet.Semaphore.GetByName(resName));

                if (thread.Priority < 1)
                    thread.Priority = random.Next(1, inputData.MaxP + 1);
                if (thread.CpuBurst < 1)
                    thread.CpuBurst = random.Next(1, inputData.MaxT + 1);
                if (thread.Quantity < 1)
                    thread.Quantity = random.Next(1, 6);

                _ = new MyThread(thread.Name, thread.Priority, thread.CpuBurst, thread.Quantity, resources);
                outputFile.WriteLine(
                    $"\tНазвание: {thread.Name}; приоритет: {thread.Priority}; время работы в квантах: {thread.CpuBurst}; сколько раз выполнить: {thread.Quantity}; названия требуемых ресурсов: {string.Join(", ", thread.ResourceNames)}");
            }

            var threadScheduler = new ThreadScheduler(inputData.Qt, inputData.Pa, outputFile);

            outputFile.WriteLine("\nПоквантовый мониторинг состояния ресурсов и потоков:");

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            threadScheduler.Execute();
            outputFile.WriteLine($"\nОбщее время работы системы: {stopwatch.ElapsedMilliseconds} мс.");

            threadScheduler.Dispose();
            
            Console.Write("Нажмите любую клавишу для завершения программы...");
            Console.ReadKey();

            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.Write("\nНажмите любую клавишу для завершения программы...");
            Console.ReadKey();
            return 1;
        }
    }
}
