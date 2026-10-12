// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Poi;

/// <summary>
/// 「查询门店」请求（官方 <c>poi/getpoi</c>）。
/// </summary>
/// <remarks>
/// <para><b>与店铺域（<c>Store</c>）的关系</b>：本域是<b>旧版微信门店接口（POI）</b>
/// （<c>/cgi-bin/poi/*</c>），<c>Store</c> 域是新版小程序店铺 API（<c>/wxa/*</c>）——两代接口并存勿合并。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Poi")]
public class MpPoiGetRequest
{
    /// <summary>获取或设置门店 ID（官方 <c>poi_id</c>，必填）。</summary>
    [JsonPropertyName("poi_id")]
    public string PoiId { get; set; } = string.Empty;
}
