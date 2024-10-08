//Необходимо разработать систему управления авиакомпаниями, которая будет симулировать обработку различных операций в авиакомпании (например, бронирование билетов, 
//    обслуживание клиентов и техническое обслуживание самолетов). В этом задании тебе нужно работать с асинхронными задачами, включая параллельное выполнение,
//    контроль ошибок и временные задержки для имитации реальной работы.

//Требования:
//Классы и объекты:

//Создай класс Airline, который управляет авиакомпанией и различными задачами.
//Создай классы для следующих задач:
//TicketBooking – симуляция процесса бронирования билетов.
//CustomerService – симуляция обработки запросов клиентов.
//Maintenance – симуляция технического обслуживания самолётов.
//Методы: Каждый класс должен содержать асинхронный метод для выполнения своей задачи:

//TicketBooking.ProcessBookingAsync(): Симулирует бронирование билетов, включая случайные задержки (от 1 до 5 секунд).
//CustomerService.HandleRequestAsync(): Симулирует обработку запросов клиентов, также включает случайные задержки (от 2 до 6 секунд).
//Maintenance.PerformMaintenanceAsync(): Симулирует обслуживание самолёта (от 3 до 7 секунд).
//Асинхронная обработка:

//В классе Airline создай метод RunOperationsAsync(), который будет запускать все вышеописанные задачи параллельно.
//Используй Task.WhenAll(), чтобы дождаться завершения всех задач.
//Добавь возможность обработать любую ошибку в одной из задач и не прервать выполнение остальных. Например, если бронирование билета завершилось с ошибкой,
//программа должна продолжить выполнение остальных операций.
//Добавь симуляцию ошибки в одном из методов (например, бронирование билетов может выбросить исключение с некоторой вероятностью).
//Реализуй отмену задач. Добавь CancellationToken и возможность отменить все задачи через 10 секунд после начала выполнения операций, если они ещё не завершены.

using System;
using System.Threading;
using System.Threading.Tasks;


public class Program
{
    public static async Task Main(string[] args)
    {
        var airline = new Airline();
        await airline.RunOperationsAsync();
    }
}
class Airline
{
    private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();


    private TicketBooking ticketBooking = new TicketBooking();
    private CustomerService customerService = new CustomerService();
    private Maintenance maintenance = new Maintenance();
    public async Task RunOperationsAsync()
    {

        var task1 = ticketBooking.ProcessBookingAsync(_cancelTokenSource.Token);
        var task2 = customerService.HandleRequestAsync(_cancelTokenSource.Token);
        var task3 = maintenance.PerformMaintenanceAsync(_cancelTokenSource.Token);

        _cancelTokenSource.CancelAfter(TimeSpan.FromSeconds(10));

        try
        {
            await Task.WhenAll(task1, task2, task3);
        }
        catch (Exception ex) when (ex is TaskCanceledException)
        {
            Console.WriteLine("Некоторые операции были отменены.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
        finally
        {
            _cancelTokenSource.Dispose();
        }
    }
}

class TicketBooking
{
    private static Random _random = new Random();
    public async Task ProcessBookingAsync(CancellationToken token)
    {
        if (token.IsCancellationRequested)
            token.ThrowIfCancellationRequested();
        else
        {
            try
            {
                var startTimeTotal = DateTime.Now;
                await Task.Delay(_random.Next(1000, 5000), token);
                if (_random.Next() < 100)
                {
                    throw new Exception("Ошибка бронирования билета.");
                }
                var endTimeTotal = DateTime.Now;
                Console.WriteLine($"Бронирование билета завершено за {endTimeTotal.Subtract(startTimeTotal).TotalSeconds} секунд.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Бронирование билета отменено.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в TicketBooking: {ex.Message}");
            }
        }
    }
}

class CustomerService
{
    private static Random _random = new Random();
    public async Task HandleRequestAsync(CancellationToken token)
    {
        if (token.IsCancellationRequested)
            token.ThrowIfCancellationRequested();
        else
        {
            try
            {
                var startTimeTotal = DateTime.Now;
                await Task.Delay(_random.Next(2000, 6000), token);
                if (_random.Next() > 500)
                {
                    throw new Exception("Ошибка обработки запроса клиента.");
                }
                var endTimeTotal = DateTime.Now;
                Console.WriteLine($"Запрос клиента обработан за {endTimeTotal.Subtract(startTimeTotal).TotalSeconds} секунд.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Обработка запроса клиента отменена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в CustomerService: {ex.Message}");
            }
        }
    }
}

class Maintenance
{
    private static Random _random = new Random();
    public async Task PerformMaintenanceAsync(CancellationToken token)
    {
        if (token.IsCancellationRequested)
            token.ThrowIfCancellationRequested();
        else
        {
            try
            {
                var startTimeTotal = DateTime.Now;
                await Task.Delay(_random.Next(3000, 7000), token);
                var endTimeTotal = DateTime.Now;
                Console.WriteLine($"Техническое обслуживание завершено за {endTimeTotal.Subtract(startTimeTotal).TotalSeconds} секунд.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Техническое обслуживание отменено.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в Maintenance: {ex.Message}");
            }
        }
    }

}