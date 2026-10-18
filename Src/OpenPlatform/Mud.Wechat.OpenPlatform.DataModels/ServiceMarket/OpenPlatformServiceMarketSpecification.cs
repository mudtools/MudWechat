// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.ServiceMarket;

/// <summary>
/// 服务规格有效期（官方 <c>spec_list</c> 数组项）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/service_market/interface/value_added/servicemarket_get_service_buyer_list.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ServiceMarket")]
public class OpenPlatformServiceMarketSpecification
{
    /// <summary>规格 ID（官方 <c>specification_id</c>）。</summary>
    [JsonPropertyName("specification_id")]
    public string? SpecificationId { get; set; }

    /// <summary>规格过期时间戳（官方 <c>expire_time</c>，Unix 秒）。</summary>
    [JsonPropertyName("expire_time")]
    public long ExpireTime { get; set; }
}
