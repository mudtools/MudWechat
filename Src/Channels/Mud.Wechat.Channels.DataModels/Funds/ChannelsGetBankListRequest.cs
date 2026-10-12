// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>搜索银行列表（<c>getbanklist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetBankListRequest
{
    /// <summary>获取或设置偏移量（官方 <c>offset</c>，选填）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置每页数据大小（官方 <c>limit</c>，选填）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置银行关键字（官方 <c>key_words</c>，选填）。</summary>
    [JsonPropertyName("key_words")]
    public string? KeyWords { get; set; }

    /// <summary>获取或设置银行类型（官方 <c>bank_type</c>，选填：1 对私银行，2 对公银行；默认对公）。</summary>
    /// <remarks><b>官方文档矛盾照录</b>：参数表说明为「1:对私银行,2:对公银行」，返回参数中 <c>bank_type</c> 说明为「1.对公，2.对私」——两处口径相反，SDK 按参数表（入参）建模。</remarks>
    [JsonPropertyName("bank_type")]
    public int? BankType { get; set; }
}
