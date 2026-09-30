// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信令牌类型常量（字符串键，与 <c>FeishuTokenTypes</c> 同模式）。
/// </summary>
/// <remarks>
/// <para>
/// 旧版组件 1.6.4 的 <c>TokenType</c> 枚举（含 <c>TenantAccessToken</c> 飞书语义）已随
/// Mud.HttpUtils 2.0.10 移除——<see cref="T:Mud.HttpUtils.Attributes.TokenAttribute"/> 的
/// TokenType 为 <c>string</c>，平台 SDK 应自建常量类。
/// </para>
/// <para>
/// 键值采用 <c>"Wechat."</c> 前缀命名空间，避免与通用 <c>TokenTypes.AccessToken</c>（"AccessToken"）
/// 在共享令牌注册表中冲突。授权企业 access_token 与自建应用共用
/// <see cref="AccessToken"/> 路由键，以 scope（authCorpId）区分缓存。
/// </para>
/// </remarks>
public static class WechatTokenTypes
{
    /// <summary>企业内访问令牌（自建应用 / 授权企业级，access_token）。</summary>
    public const string AccessToken = "Wechat.AccessToken";

    /// <summary>服务商访问令牌（provider_access_token）。</summary>
    public const string ProviderAccessToken = "Wechat.ProviderAccessToken";

    /// <summary>第三方/服务商套件令牌（suite_access_token）。</summary>
    public const string SuiteAccessToken = "Wechat.SuiteAccessToken";
}
