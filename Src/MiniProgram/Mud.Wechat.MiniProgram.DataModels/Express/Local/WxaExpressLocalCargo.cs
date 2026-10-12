// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送货物信息（<c>cargo</c> 内嵌对象；下单 / 预下单 / 重新下单复用）。</summary>
/// <remarks>
/// 官方数值口径：金额单位为<b>元</b>、重量单位为 <b>kg</b>、尺寸单位为 <b>cm</b>，均精确到小数点后两位；
/// 取值范围 <c>goods_value</c> (0-5000]、<c>goods_height</c> (0-45]、<c>goods_width</c> (0-50]、
/// <c>goods_length</c> (0-65]、<c>goods_weight</c> (0-50]。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalCargo
{
    /// <summary>货物价格（<c>goods_value</c>，必填；单位：元，范围 (0-5000]）。</summary>
    [JsonPropertyName("goods_value")]
    public decimal GoodsValue { get; set; }

    /// <summary>货物高度（<c>goods_height</c>，可选；单位：cm，范围 (0-45]）。</summary>
    [JsonPropertyName("goods_height")]
    public decimal? GoodsHeight { get; set; }

    /// <summary>货物宽度（<c>goods_width</c>，可选；单位：cm，范围 (0-50]）。</summary>
    [JsonPropertyName("goods_width")]
    public decimal? GoodsWidth { get; set; }

    /// <summary>货物长度（<c>goods_length</c>，可选；单位：cm，范围 (0-65]）。</summary>
    [JsonPropertyName("goods_length")]
    public decimal? GoodsLength { get; set; }

    /// <summary>货物重量（<c>goods_weight</c>，必填；单位：kg，范围 (0-50]）。</summary>
    [JsonPropertyName("goods_weight")]
    public decimal GoodsWeight { get; set; }

    /// <summary>货物详情（<c>goods_detail</c>，可选），见 <see cref="WxaExpressLocalGoodsDetail"/>。</summary>
    [JsonPropertyName("goods_detail")]
    public WxaExpressLocalGoodsDetail? GoodsDetail { get; set; }

    /// <summary>货物取货信息（<c>goods_pickup_info</c>，可选；用于骑手到店取货，最长不超过 100 个字符）。</summary>
    [JsonPropertyName("goods_pickup_info")]
    public string? GoodsPickupInfo { get; set; }

    /// <summary>品类一级类目（<c>cargo_first_class</c>，必填；官方枚举，如「美食夜宵」「日用百货」「果蔬生鲜」等）。</summary>
    [JsonPropertyName("cargo_first_class")]
    public string? CargoFirstClass { get; set; }

    /// <summary>品类二级类目（<c>cargo_second_class</c>，必填）。</summary>
    [JsonPropertyName("cargo_second_class")]
    public string? CargoSecondClass { get; set; }
}
