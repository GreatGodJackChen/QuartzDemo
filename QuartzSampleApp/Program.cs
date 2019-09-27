﻿using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace QuartzSampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            RunProgramRunExample().GetAwaiter().GetResult();
            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
        }
        private static async Task RunProgramRunExample()
        {
            try
            {
                // 从工厂中获取调度程序实例
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                // 开启调度器
                await scheduler.Start();

                // 定义这个工作，并将其绑定到我们的IJob实现类
                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                // 触发作业立即运行，然后每10秒重复一次，无限循环
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();

                // 告诉Quartz使用我们的触发器来安排作业
                await scheduler.ScheduleJob(job, trigger);

                // 等待60秒
                await Task.Delay(TimeSpan.FromSeconds(60));

                // 关闭调度程序
                await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                await Console.Error.WriteLineAsync(se.ToString());
            }
        }
    }
}
