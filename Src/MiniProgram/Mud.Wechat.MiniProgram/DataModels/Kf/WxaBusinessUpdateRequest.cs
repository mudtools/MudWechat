// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Kf;

/// <summary>更新客服子商户信息请求体（<c>POST /cgi-bin/business/update</c>）。</summary>
/// <remarks>官方文档：<c>kf-mgnt/kf-management/api_updatebusiness.html</c>。<c>mch_id</c> 必填定位子商户，品牌字段为待更新内容。</remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class WxaBusinessUpdateRequest
{
    /// <summary>商户号（<c>mch_id</c>，必填）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>品牌名称（<c>brand_name</c>）。</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>品牌图标（<c>brand_icon</c>；图片永久素材媒体 ID）。</summary>
    [JsonPropertyName("brand_icon")]
    public string? BrandIcon { get; set; }
}
