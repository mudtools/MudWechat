// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>查询支持同城配送的城市请求体（<c>POST /cgi-bin/express/intracity/getcity</c>）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/same_city_distribution/api_intracity_getcity"/>
/// （接口英文名 <c>intracity_getcity</c>，2026-10-12 核验）。
/// </para>
/// <para>
/// <c>service_trans_id</c> <b>不传</b>即返回<b>全部运力</b>的支持城市；传值则只返回该运力的结果
/// （官方示例取值：<c>SFTC</c> 顺丰同城、<c>DADA</c> 达达）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetCityRequest
{
    /// <summary>运力 ID（<c>service_trans_id</c>，可选；不传则返回全部运力结果）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }
}
