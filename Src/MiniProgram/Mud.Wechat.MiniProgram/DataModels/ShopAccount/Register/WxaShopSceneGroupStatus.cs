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
/// 场景接入审核条目（官方 <c>/shop/register/check</c> 回包 <c>data.scene_group_list[]</c>）。
/// </summary>
/// <remarks>
/// <para><b>本层 <c>status</c> 取值表与 <c>/shop/audit/result</c> 的不同</b>：本页 0 审核中 / 1 审核完成 /
/// 2 审核失败 / 3 未审核；audit/result 为 0 审核中 / 1 审核成功 / 9 审核拒绝 ⇒ 不得共用判定器。</para>
/// <para><b>官方历史脏值留档</b>（回包表原文注）：「由于历史原因可能出现 0 和 1 的 status，视为已接入即可，
/// 后续会自动流转到 2」⇒ 本层 <c>0</c>/<c>1</c> 混用期内的「已接入」判据不能只取 <c>1</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopSceneGroupStatus
{
    /// <summary>场景枚举（<c>group_id</c>，官方回包表类型列写作 object 但示例与语义均为数字 <c>1</c>；<c>1</c> 视频号、公众号场景）。</summary>
    [JsonPropertyName("group_id")]
    public long? GroupId { get; set; }

    /// <summary>场景名称（<c>name</c>，string；官方示例「视频号、公众号场景」）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>审核状态（<c>status</c>，number）：<c>0</c> 审核中 / <c>1</c> 审核完成 / <c>2</c> 审核失败 / <c>3</c> 未审核。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>审核理由（<c>reason</c>，string；官方示例「小程序没有售后和客服功能，暂时无法使用视频号场景。」）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>场景相关审核结果（<c>scene_group_ext_list</c>，array），见 <see cref="WxaShopSceneGroupExt"/>。</summary>
    [JsonPropertyName("scene_group_ext_list")]
    public List<WxaShopSceneGroupExt>? SceneGroupExtList { get; set; }
}
