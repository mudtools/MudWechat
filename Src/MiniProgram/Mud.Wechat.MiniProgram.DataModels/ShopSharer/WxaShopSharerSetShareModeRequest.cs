// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.ShopSharer;

/// <summary>
/// 设置小程序分享模式请求（<c>POST /shop/sharer/set_share_mode</c>，请求体只有一支分享模式键）。
/// </summary>
/// <remarks>
/// <para><b>官方本页键名自相矛盾（本域最争议的一处，裁决留档）</b>：「请求参数示例」写
/// <c>{"share_mode": 2}</c>，而「请求参数说明」表写 <c>sharer_mode</c>。本仓取<b>示例键名
/// <c>share_mode</c></b>建模（口径同 Novel 域 NV4 的「示例形态优先」裁决：示例是可直接投递的 JSON 实体，
/// 表侧 <c>sharer_mode</c> 判为官方笔误并在此留档）。宿主若遇官方按表拒收，须以实际投递验证为准回改本键并同批更新守卫 SHOPSH5。</para>
/// <para><b>取值语义</b>：<c>1</c> 分享员模式（默认）、<c>2</c> 分享归因模式。切到 <c>2</c> 后小程序对分享员
/// <b>不可见</b>、微信侧停止统计展示该小程序分享员带货数据（官方原文）⇒ 影响本域其余 7 支数据的可见面，非纯配置项。</para>
/// <para><b>30 天冷却</b>：两次更改间隔不得短于 30 天，违反返回 <c>1310005</c> ⇒ 本端点<b>严禁</b>放进自动重试管道。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopSharer")]
public class WxaShopSharerSetShareModeRequest
{
    /// <summary>
    /// 分享模式（<c>share_mode</c>，number）：<c>1</c> 分享员（默认）、<c>2</c> 分享归因。
    /// 官方参数表误写 <c>sharer_mode</c> ⇒ 本仓照请求体示例钉 <c>share_mode</c>（见类型 remarks 裁决）。
    /// </summary>
    [JsonPropertyName("share_mode")]
    public int? ShareMode { get; set; }
}
