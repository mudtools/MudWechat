// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.MarketingGoodsSubsidyActivity;

/// <summary>上传小店活动素材请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384395"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActMaterialUploadRequest
{
    /// <summary>【请求业务单据】 商户添加业务代理信息凭据号，商户侧需保持唯一性。可包含英文字母，数字，&amp;#124;，_， _，-等内容，不允许出现其他不合法符号。_（官方字段路径 <c>out_request_no</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }

    /// <summary>【生成物料码数量】 生成物料码数量（官方字段路径 <c>material_num</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("material_num")]
    public long? MaterialNum { get; set; }

}

/// <summary>上传小店活动素材响应·【成功的物料码列表】 成功的物料码列表。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384395"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActMaterialUploadResponseMaterialListItem
{
    /// <summary>【物料码ID】 物料码ID（官方字段路径 <c>material_list.material_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("material_id")]
    public string? MaterialId { get; set; }

    /// <summary>【物料码链接】 物料码链接, 用于生成活动物料二维码（官方字段路径 <c>material_list.material_url</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("material_url")]
    public string? MaterialUrl { get; set; }

}

/// <summary>上传小店活动素材响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012384395"/></summary>
[HttpJsonSerializable(SerializerClassName = "MarketingGoodsSubsidyActivity")]
public class RetailStoreActMaterialUploadResponse : WechatPayResponse
{
    /// <summary>【成功的物料码列表】 成功的物料码列表（官方字段路径 <c>material_list</c>，选填，类型 <c>array[object]</c>）。</summary>
    [JsonPropertyName("material_list")]
    public List<RetailStoreActMaterialUploadResponseMaterialListItem>? MaterialList { get; set; }

}
