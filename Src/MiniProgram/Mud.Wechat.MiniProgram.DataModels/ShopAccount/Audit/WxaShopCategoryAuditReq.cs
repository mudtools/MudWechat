// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopAccount;

/// <summary>
/// 类目审核请求体（官方 <c>audit_req</c> 一层，<c>audit_category</c> 侧）。
/// </summary>
/// <remarks>
/// <para><b>官方本页的幂等陷阱</b>（原文）：「这个上传类目资质的接口，如果上传的类目是已经审核通过的，
/// 该接口会返回错误码 <c>1050003</c>」⇒ 重复提审已通过的类目不是 no-op，而是报错，宿主不得放进重试管道。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopCategoryAuditReq
{
    /// <summary>
    /// 营业执照或组织机构代码证（<c>license</c>，string 数组，<b>必填</b>）。
    /// 官方本页说明列只写「图片 url」（未像品牌页那样同时接受 media_id），图片须经「上传图片」接口换取临时链接，
    /// 上传后微信侧转永久链接。
    /// </summary>
    [JsonPropertyName("license")]
    public List<string>? License { get; set; }

    /// <summary>类目与资质材料（<c>category_info</c>，object，必填），见 <see cref="WxaShopCategoryAuditInfo"/>。</summary>
    [JsonPropertyName("category_info")]
    public WxaShopCategoryAuditInfo? CategoryInfo { get; set; }

    /// <summary>
    /// 类目使用场景（<c>scene_group_list</c>，enum 数组，<b>必填</b>）。
    /// 官方取值：<c>1</c> 视频号、<c>3</c> 订单中心（非视频号订单中心）；
    /// 官方原文同样警示「未明确开通此场景的商家请勿传值」「组件开通流程中以及未接入场景时请保持为空
    /// <c>"scene_group_list":[]</c>」。
    /// </summary>
    [JsonPropertyName("scene_group_list")]
    public List<int>? SceneGroupList { get; set; }
}
