// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>查询支行列表（<c>getsubbranch</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetSubBranchRequest
{
    /// <summary>获取或设置银行编码（官方 <c>bank_code</c>，必填；通过查询银行信息或者搜索银行信息获取）。</summary>
    [JsonPropertyName("bank_code")]
    public string BankCode { get; set; } = string.Empty;

    /// <summary>获取或设置城市编号（官方 <c>city_code</c>，必填；通过查询城市列表获取；官方示例写作字符串 <c>"571"</c>，类型按官方参数表为 number）。</summary>
    [JsonPropertyName("city_code")]
    public int CityCode { get; set; }

    /// <summary>获取或设置偏移量（官方 <c>offset</c>，选填）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置限制个数（官方 <c>limit</c>，选填）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}
