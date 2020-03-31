using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ator.DbEntity.SqlSuger;
using Ator.Utility.CacheService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using AutoMapper;
using Ator.Model;
using System.Reflection;
using Ator.DbEntity.Factory;
using SqlSugar;

namespace Ator.Site
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //ע��SqlSuger����
            SysConfig.InitConfig();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //����ڴ滺��
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            //���Session
            services.AddSession();

            //��ӻ������ע��
            services.AddSingleton(typeof(Utility.CacheService.ICacheService), typeof(MemoryCacheService));

            //ͨ������ע����ַ���
            services.AddServiceScoped();

            #region ��֤����
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                }).AddCookie(options =>
                {
                //Cookie��middleware����
                options.LoginPath = new PathString("/Admin/Home/Login");
                    options.AccessDeniedPath = new PathString("/Admin/Home/Login");
                //options.ExpireTimeSpan = //��Ч��
            });
            #endregion

            //ע��AutoMapper����
            services.AddAutoMapper(typeof(AutoMapperProfileConfiguration));

            //ע��sqlSuper���ݲ��������ڴ���Ŀ����2�в������ݷ�ʽ����һ�ַ�װ��Ator.DAL�㣬���������ط�ʹ��var db=SugarHandler.Instance();
            services.AddSqlSugarClient<DbFactory>((sp, op) =>
            {
                op.ConnectionString = sp.GetService<IConfiguration>().GetConnectionString("MySQLConn");
                op.DbType = DbType.MySql;
                op.IsAutoCloseConnection = true;
                op.InitKeyType = InitKeyType.Attribute;
                op.IsShardSameThread = true;
            });

            //ע��mvc����������ͼ
            services.AddControllersWithViews();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();//ʹ�þ�̬�ļ�

            app.UseRouting();//ʹ��·��

            app.UseAuthorization();//ʹ����֤

            app.UseAuthentication();//��֤����

            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto }); //���IP��ȡ


            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "areas", "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
