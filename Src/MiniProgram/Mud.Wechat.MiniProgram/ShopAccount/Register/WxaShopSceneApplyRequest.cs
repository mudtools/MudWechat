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
/// 场景接入申请请求（<c>POST /shop/register/apply_scene</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方本页「参数表」与「请求示例」键名不一致</b>（原文差异，非本仓笔误）：
/// 请求示例写作 <c>{"scene_group_id": 1}</c>，参数表却列作 <c>scene_group</c> ⇒
/// 本仓以<b>请求示例的 <c>scene_group_id</c> 为准</b>落地（示例即官方给出的可运行 JSON），
/// 差异由守卫 SHOPA5 锁定并在此留档；宿主若被拒可临时改试 <c>scene_group</c>。</para>
/// <para><b>本端点与 <c>/shop/register/apply</c> 不是一回事</b>：apply 是「开通自定义版交易组件」的账号级动作，
/// apply_scene 是「申请某个使用场景」的场景级动作（取值 <c>1</c> 视频号）⇒ 后者需先完成前者。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopSceneApplyRequest
{
    /// <summary>
    /// 场景组 id（<c>scene_group_id</c>，number，<b>必填</b>）。
    /// 官方本页参数表原文「1:视频号」；<c>/shop/register/check</c> 与两支提审的
    /// <c>scene_group_list</c> 用同一套枚举（该处官方另列 <c>3</c> 订单中心）。
    /// </summary>
    [JsonPropertyName("scene_group_id")]
    public long? SceneGroupId { get; set; }
}
