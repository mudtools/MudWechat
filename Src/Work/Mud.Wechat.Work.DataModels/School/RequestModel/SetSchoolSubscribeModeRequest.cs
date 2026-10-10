// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School;

/// <summary>
/// 设置关注「学校通知」的模式请求体（<c>/cgi-bin/externalcontact/set_subscribe_mode</c>）。
/// <para>官方业务限制：企业必须完成验证才可调用，否则返回错误码 43009；
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "School")]
public class SetSchoolSubscribeModeRequest
{
    /// <summary>
    /// 获取或设置关注模式（官方必填）：1 - 可扫码填写资料加入、2 - 禁止扫码填写资料加入。
    /// </summary>
    [JsonPropertyName("subscribe_mode")]
    public int? SubscribeMode { get; set; }
}
