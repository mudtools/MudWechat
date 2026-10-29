// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PayTool;

/// <summary>
/// 获取收款订单列表请求体（<c>/cgi-bin/paytool/get_order_list</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>签名三要素</b>：<see cref="NonceStr"/> / <see cref="Ts"/> / <see cref="Sig"/>，
/// 算法见官方 <see href="https://developer.work.weixin.qq.com/document/path/98768">path 98768 签名算法</see>。
/// </para>
/// <para>
/// <b>官方权限口径</b>：本端点页面标注「无特殊权限」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class GetPayToolOrderListRequest
{
    /// <summary>
    /// 获取或设置业务类型（官方可选）：<c>1</c>-普通第三方应用 / <c>2</c>-代开发应用 / <c>3</c>-行业解决方案；
    /// 不填表示不限业务类型。
    /// </summary>
    [JsonPropertyName("business_type")]
    public int? BusinessType { get; set; }

    /// <summary>
    /// 获取或设置查询起始时间（官方可选，unix 时间戳）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置查询结束时间（官方可选，unix 时间戳）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置分页游标（官方可选）。
    /// <para>官方口径：字符串类型，由上一次调用返回；<b>首次调用不填</b>。</para>
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置分页条数（官方必填）：预期请求的数据量，取值范围 <b>1 ~ 2000</b>。
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>
    /// 获取或设置随机字符串（官方必填，长度要求在 32 字节以内）。
    /// <para>官方约束：需保证 15 分钟内不能重复。</para>
    /// </summary>
    [JsonPropertyName("nonce_str")]
    public string NonceStr { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 unix 时间戳（中国时区，精确到秒，官方必填）。
    /// <para>官方约束：业务系统的机器时间与腾讯的时间相差不能超过 15 分钟。</para>
    /// </summary>
    [JsonPropertyName("ts")]
    public long Ts { get; set; }

    /// <summary>
    /// 获取或设置数字签名（官方必填）。
    /// </summary>
    [JsonPropertyName("sig")]
    public string Sig { get; set; } = string.Empty;
}
