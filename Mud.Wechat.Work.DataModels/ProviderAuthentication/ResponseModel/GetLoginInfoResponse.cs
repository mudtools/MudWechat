// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ProviderAuthentication;

/// <summary>
/// 获取登录用户信息响应体（<c>/cgi-bin/service/get_login_info</c>，官方文档 91154）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方契约特例（errcode 缺省成功语义）</b>：官方 91154「因历史原因，调用失败时才返回 errcode，
/// 无 errcode 字段视为成功」。本类型直接继承 <see cref="WechatWorkResponse"/>
/// （<c>errcode</c> 为 <c>int</c>、缺省 0）：报文缺失 errcode 字段时反序列化保持 0，
/// <c>IsSuccess</c> / 判错出口即按成功处理——该判定路径由守卫 PL1 锁定，
/// <b>不得</b>改为可空 errcode、自定义 IsSuccess 或自建判定器。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProviderAuthentication")]
public class GetLoginInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置授权方企业（扫码登录的企业）的 corpid。
    /// </summary>
    [JsonPropertyName("corpid")]
    public string? CorpId { get; set; }

    /// <summary>
    /// 获取或设置登录用户的 userid（企业成员唯一标识）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>
    /// 获取或设置登录用户的姓名。
    /// </summary>
    /// <remarks>
    /// <para>官方 2020-06-30 起对服务商不再返回真实姓名，该字段返回 userid（历史兼容字段），勿作真实姓名消费。</para>
    /// </remarks>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置登录用户头像 url。
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// 获取或设置性别：1 - 男性，2 - 女性（官方示例按字符串传输，如 <c>"1"</c>，故本模型以字符串承载）。
    /// </summary>
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }
}
