// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.BusinessCircle;

/// <summary>商圈会员待积分状态查询响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012534994"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012474129"/></summary>
[HttpJsonSerializable(SerializerClassName = "BusinessCircle")]
public class BusinessCirclePointsCommitStateResponse : WechatPayResponse
{
    /// <summary>【商圈会员待积分状态】 商圈会员待积分状态（官方字段路径 <c>points_commit_status</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("points_commit_status")]
    public string? PointsCommitStatus { get; set; }

}
