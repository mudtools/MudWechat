// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.QrcodeJump;

/// <summary>
/// 获取已设置的二维码规则（<c>POST /cgi-bin/wxopen/qrcodejumpget</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>本组参数仅「扫服务号二维码打开小程序」场景需要</b>（官方原文：获取「扫普通二维码打开小程序」规则时
/// <b>无需任何入参</b>）——但官方字段表把这几个参数标为「必填」，两处矛盾已在官方页并存（照录，
/// SDK 以可空超集承载，由调用方按场景携带）。
/// </para>
/// <para>
/// 官方「服务号调用说明」原文：服务商调用本接口须先获得服务号授权<b>权限集 id 为 3</b> 的权限集，
/// 否则返回 <c>61007</c>；服务号须<b>先关联小程序</b>才可调用（关联入口：<c>linkMiniprogram</c> 接口，
/// 或公众号管理后台「广告与服务 — 小程序管理」）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "QrcodeJump")]
public class MpQrcodeJumpGetRequest
{
    /// <summary>获取或设置小程序的 appid（官方 <c>appid</c>；仅获取「扫服务号二维码打开小程序」规则时需传）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置查询类型（官方 <c>get_type</c>，默认 <c>0</c>；取值见 <c>MpQrcodeJumpGetTypes</c>）。
    /// 属性名为 <c>QueryType</c>：避免与 <see cref="object.GetType()"/> 同名（CS0108）。</summary>
    [JsonPropertyName("get_type")]
    public int? QueryType { get; set; }

    /// <summary>获取或设置前缀列表（官方 <c>prefix_list</c>，<c>get_type = 1</c> 时必传，<b>最多 200 个</b>）。</summary>
    [JsonPropertyName("prefix_list")]
    public List<string>? PrefixList { get; set; }

    /// <summary>获取或设置页码（官方 <c>page_num</c>，<c>get_type = 2</c> 时必传，从 1 开始）。</summary>
    [JsonPropertyName("page_num")]
    public int? PageNumber { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，<c>get_type = 2</c> 时必传，<b>最大 200</b>）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}
