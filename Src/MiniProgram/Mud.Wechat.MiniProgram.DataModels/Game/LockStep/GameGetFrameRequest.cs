// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>分片拉取对局游戏帧请求。官方为 POST + JSON 请求体；帧区间是左闭右开。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameFrame.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameGetFrameRequest
{
    /// <summary>官方原文「填 onGameEnd 接口返回给开发者的 gameAccessInfo」（必填）。</summary>
    [JsonPropertyName("access_info")]
    public string? AccessInfo { get; set; }

    /// <summary>待获取帧的起始 frame_id（官方必填）。</summary>
    [JsonPropertyName("begin_frame_id")]
    public int BeginFrameId { get; set; }

    /// <summary>待获取帧的终止 frame_id，左闭右开区间（官方必填）。</summary>
    [JsonPropertyName("end_frame_id")]
    public int EndFrameId { get; set; }
}
