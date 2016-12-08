using System;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;


namespace ConsoleApplication
{

    public class Timeline
    {

        public Timeline(string message){
            Time = DateTime.Now;
            Message=message;
        }
        public DateTime Time{ get; set;}

        public string Message{ get; set;}

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
                .ForMember(dst => dst.Message,src => src.MapFrom(s=> s.Message ?? "Hi"));
        }

    }

    public interface ITimelinePrintService
    {
         void Print(Timeline timeline) ;
    }

    public class TimelineConsolePrintService : ITimelinePrintService
    {
        private readonly IMapper _mapper;

        public TimelineConsolePrintService(IMapper mapper) { // ← Mapperオブジェクトがインジェクションされる
            // 自分のプロパティに保持
            this._mapper = mapper;
        }

        public void Print(Timeline timeline)
        {
            TimelineDto timelineDto =this._mapper.Map<Timeline,TimelineDto>(timeline);
            Console.WriteLine(timelineDto.Time+":"+timelineDto.Message);
        }
    }

    public class Program
    {

        public static void Main(string[] args)
        {
            var services =  new ServiceCollection();

            services.AddAutoMapper(cfg =>{
                    cfg.AddProfile(new AutoMapperExampleProfile());
            });
            
            var serviceProvider = services.AddSingleton<IMapper, Mapper>()
                .AddSingleton<ITimelinePrintService,TimelineConsolePrintService>()
                .BuildServiceProvider();
            
            var printService = serviceProvider.GetService<ITimelinePrintService>();
            printService.Print(new Timeline(args.Length == 1 ? args[0]:null));
        }
    }
}
