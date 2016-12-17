using System;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using System.IO;
using System.Linq;


namespace ConsoleApplication
{
    public enum TIMELINE_STATUS {
        FIRST,CONTINUE
    }

    public class TimeLineHeader
    {
        public TIMELINE_STATUS Status{get;set;} = TIMELINE_STATUS.FIRST;

        public string UserName{get; set;} = "";

        public int LastIndex{get; set;} = 1;

        public string TargetYaml{get; set;}

    }

    public class Timeline
    {

        public Timeline(string message){
            Time = DateTime.Now;
            Message=message;
        }

        public DateTime Time{ get; set;}

        public string Message{ get; set;}

        public int Index {get;set;}
    }

    public class TimelineDto
    {
        public string Time{get;set;}

        public string Message{get;set;}

    }

    public class AutoMapperExampleProfile: Profile{

        public AutoMapperExampleProfile()
        {
            CreateMap<Timeline,TimelineDto>()
                .ForMember(dst => dst.Time,src => src.MapFrom(s=>s.Time.ToString()))
                .ForMember(dst => dst.Message,src => src.MapFrom(s=>s.Message ?? "Hi"));
        }

    }

    public interface ITimelinePrintService
    {
         void Print(Timeline timeline) ;
    }

    public class TimelineConsolePrintService : ITimelinePrintService
    {
        private readonly IMapper _mapper;
        private readonly Func<TimeLineHeader> _timeLineHeaderSupplier;
        public TimelineConsolePrintService(IMapper mapper,Func<TimeLineHeader> supplier) { 
            this._mapper = mapper;
            this._timeLineHeaderSupplier = supplier;
        }

        public void Print(Timeline timeline)
        {
            var timeLineHeader = _timeLineHeaderSupplier();
            timeline.Index = timeLineHeader.LastIndex;
            TimelineDto timelineDto =this._mapper.Map<Timeline,TimelineDto>(timeline);
            Console.WriteLine(timelineDto.Time+":"+timelineDto.Message);

            if (timeLineHeader.UserName == "")
                    timeLineHeader.UserName = "message";
                        
            if (timeLineHeader.Status == TIMELINE_STATUS.FIRST)
            {
                var file = File.Create(timeLineHeader.TargetYaml);
                using(StreamWriter  timelineWriter = new StreamWriter(file))
                {
                    timelineWriter.WriteLine("timelines:");
                    timelineWriter.WriteLine("  - timelines1:"+timelineDto.Time);
                    timelineWriter.WriteLine("      {0}:{1}",timeLineHeader.UserName,timelineDto.Message);
                    timelineWriter.Dispose();
                }
            }else{
                using(StreamWriter  timelineWriter2 = new StreamWriter(File.Open(timeLineHeader.TargetYaml,FileMode.Append)))
                {
                    timelineWriter2.WriteLine("  - timelines{0}:{1}", ++(timeLineHeader.LastIndex),timelineDto.Time);
                    timelineWriter2.WriteLine("      {0}:{1}",timeLineHeader.UserName,timelineDto.Message);
                    timelineWriter2.Dispose();
                }
            }
        }
    }


    public class Program
    {
         
        public static TimeLineHeader createTimeLineHeader(string target,string userName)
        {
            var outputFile = DateTime.Now.ToString("yyyyMM")+".timelines";
            if(!System.IO.Directory.Exists(target))
            {
                System.IO.Directory.CreateDirectory(target);
                return new TimeLineHeader { UserName = userName,TargetYaml = target + Path.DirectorySeparatorChar+ outputFile };
            }else if (!System.IO.File.Exists(target + Path.DirectorySeparatorChar+ outputFile))
            {
                return new TimeLineHeader { UserName = userName,TargetYaml = target + Path.DirectorySeparatorChar+ outputFile };
            }
            var input = (from line in File.ReadLines(target + Path.DirectorySeparatorChar+ outputFile)
                where line.StartsWith("  - timelines")
                select new
                {
                    File = target + Path.DirectorySeparatorChar+ outputFile,
                    Count = line.Split(':')[0].Substring(13)
                }).Last();
            if (input == null){
                 throw new System.InvalidOperationException("timelife file is invalid");
            }
            return new TimeLineHeader{Status =TIMELINE_STATUS.CONTINUE,UserName = userName,TargetYaml =input.File,LastIndex=int.Parse(input.Count)};
    }
        public static void Main(string[] args)
        {

            var services =  new ServiceCollection();

            services.AddAutoMapper(cfg =>{
                    cfg.AddProfile(new AutoMapperExampleProfile());
            });
           
            var serviceProvider = services
                .AddSingleton<Func<TimeLineHeader>>(() =>createTimeLineHeader(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar+"data","kaoru"))
                .AddSingleton<IMapper, Mapper>()
                .AddSingleton<ITimelinePrintService,TimelineConsolePrintService>()
                .BuildServiceProvider();
            
            var printService = serviceProvider.GetService<ITimelinePrintService>();
            printService.Print(new Timeline(args.Length == 1 ? args[0]:null));
        }
    }
}
