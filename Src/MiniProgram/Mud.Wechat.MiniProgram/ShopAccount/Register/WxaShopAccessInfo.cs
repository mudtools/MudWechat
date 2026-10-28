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
/// 接入状态内容体（官方 <c>/shop/register/check</c> 回包 <c>data</c>）。
/// </summary>
/// <remarks>
/// <para><b>「接入任务完成度」与「场景接入审核」是同一 <c>data</c> 下的两条独立线</b>：
/// 前者是 <c>access_info</c> 的 0/1 布尔位，后者是 <c>scene_group_list</c> 的审核状态数组 ⇒
/// 全部 <c>*_finished = 1</c> 不代表场景（视频号）可用，反之亦然。</para>
/// <para><b>键名与 <c>finish_access_info</c> 的请求侧不是一对一</b>：申报侧用枚举整数（6/7/8/9/10/11/19/20），
/// 回读侧用 <c>*_success</c> / <c>*_finished</c> 具名位 ⇒ 两套建模不合并。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopAccount")]
public class WxaShopAccessInfo
{
    /// <summary>接入任务相关信息（<c>access_info</c>，object），见 <see cref="WxaShopAccessTaskInfo"/>。</summary>
    [JsonPropertyName("access_info")]
    public WxaShopAccessTaskInfo? AccessInfo { get; set; }

    /// <summary>
    /// 场景接入相关列表（<c>scene_group_list</c>，官方回包表写作 <c>object array</c>），
    /// 见 <see cref="WxaShopSceneGroupStatus"/>。官方原文注明「由于历史原因可能出现 0 和 1 的 status，
    /// 视为已接入即可，后续会自动流转到 2」。
    /// </summary>
    [JsonPropertyName("scene_group_list")]
    public List<WxaShopSceneGroupStatus>? SceneGroupList { get; set; }
}
