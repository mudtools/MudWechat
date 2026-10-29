// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Mud.Wechat.Demo.Shared;
using Mud.Wechat.Work.OrgApprovalDemo.Configuration;

namespace Mud.Wechat.Work.OrgApprovalDemo.Data;

/// <summary>
/// 库文件路径解析（方案 §5.1 / §8.2）：缺省落 <c>{AppContext.BaseDirectory}/app_data/org-demo.db</c>
/// —— 随 <c>**/bin/</c> 一起被 gitignore；唯一覆盖入口是 <c>MUDWECHAT_DEMO_WORK_ORG_DB_PATH</c>
/// 或等价配置键 <c>OrgDemo:DbPath</c>（变量优先，两处都不接受占位符）。
/// </summary>
/// <remarks>
/// <b>库内明文存成员手机号与邮箱</b>：绝对路径在共享机器上等于多给一层定位信息，故 <see cref="Describe"/>
/// 只回显文件名与大小，<c>/healthz</c> 不得回显完整路径。
/// </remarks>
public static class OrgDbPath
{
    /// <summary>覆盖用的变量名（不含 <see cref="DemoEnv"/> 前缀）。</summary>
    public const string EnvName = "WORK_ORG_DB_PATH";

    private const string DirectoryName = "app_data";
    private const string FileName = "org-demo.db";

    /// <summary>解析并确保父目录存在，返回绝对路径。</summary>
    public static string Resolve(IConfiguration configuration)
    {
        var path = DemoEnv.Optional(EnvName, configuration, DemoConfig.DbPathKey)
                   ?? Path.Combine(AppContext.BaseDirectory, DirectoryName, FileName);

        // 环境变量给的是裸文件名时按程序目录补齐，避免落进「当前工作目录」这种随启动方式漂移的位置。
        if (!Path.IsPathRooted(path))
        {
            path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
        }

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        return path;
    }

    /// <summary>连接串（<c>Cache=Shared</c>：同进程内多连接共享页缓存，后台同步与读端点并发才不互相踢）。</summary>
    public static string ConnectionString(string path) => $"Data Source={path};Cache=Shared";

    /// <summary>供 <c>/healthz</c> 回显的摘要：<b>只有文件名与大小</b>，不含绝对路径。</summary>
    public static string Describe(string path)
    {
        var info = new FileInfo(path);
        return info.Exists
            ? $"{info.Name} · {info.Length} bytes"
            : $"{info.Name} · (尚未创建)";
    }
}
