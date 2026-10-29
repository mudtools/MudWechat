// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.ArticleDemo.Configuration;

/// <summary>
/// 配置面登记（方案 §9）：每个可覆盖项都是「一个变量名 + 一个配置键」的成对来源，
/// <b>变量优先、JSON 其次</b>，两处共用同一套占位符判定（占位符 = 未配置）。
/// <para>
/// 写闸<b>刻意不在这里</b>：<c>MUDWECHAT_DEMO_ALLOW_WRITE</c> 是唯一放行入口（基座契约，见
/// <c>Demos/README.md</c> §3）。给不可逆写操作再开一条文件里的开关，等于把「默认关闭」变成
/// 「换台机器就默认打开」，且不再能被单点 grep 到。
/// </para>
/// </summary>
public static class DemoConfig
{
    /// <summary>配置节名。</summary>
    public const string Section = "WechatDemo";

    /// <summary>AppId 的变量名（不含 <c>MUDWECHAT_DEMO_</c> 前缀）。</summary>
    public const string AppIdEnv = "MP_APP_ID";

    /// <summary>AppId 的配置键。</summary>
    public const string AppIdKey = Section + ":AppId";

    /// <summary>AppSecret 的变量名。</summary>
    public const string AppSecretEnv = "MP_APP_SECRET";

    /// <summary>AppSecret 的配置键。</summary>
    public const string AppSecretKey = Section + ":AppSecret";

    /// <summary>库路径的配置键（变量名见 <c>DbPathResolver.EnvName</c>）。</summary>
    public const string DbPathKey = Section + ":DbPath";
}
