// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送货品明细项（<c>cargo.goods_detail.goods</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalGood
{
    /// <summary>货物数量（<c>good_count</c>，必填）。</summary>
    [JsonPropertyName("good_count")]
    public int GoodCount { get; set; }

    /// <summary>货品名称（<c>good_name</c>，必填）。</summary>
    [JsonPropertyName("good_name")]
    public string? GoodName { get; set; }

    /// <summary>货品单价（<c>good_price</c>，可选；精确到小数点后两位）。</summary>
    [JsonPropertyName("good_price")]
    public decimal? GoodPrice { get; set; }

    /// <summary>货品单位（<c>good_unit</c>，可选；最长不超过 20 个字符）。</summary>
    [JsonPropertyName("good_unit")]
    public string? GoodUnit { get; set; }
}
