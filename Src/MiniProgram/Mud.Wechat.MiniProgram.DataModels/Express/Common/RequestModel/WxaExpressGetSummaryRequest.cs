// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>拉取理赔摘要请求体（<c>POST /wxa/business/insurance_freight/getsummary</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/weixin-express/freight/api_insurance_freight_getsummary"/>（接口英文名 <c>insurance_freight_getsummary</c>）；可查询当前保费、投保单量、理赔单量、账号余额等。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressGetSummaryRequest
{
    /// <summary>查询开始时间（<c>begin_time</c>，必填；秒级时间戳）。</summary>
    [JsonPropertyName("begin_time")]
    public long BeginTime { get; set; }

    /// <summary>查询结束时间（<c>end_time</c>，必填；秒级时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }
}
