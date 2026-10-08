// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Checkin;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「打卡」模块打卡记录域第三方应用 SDK。
/// <para>
/// 承载「获取打卡记录数据」的第三方文档口径旧字段结构（官方文档 94205：
/// agency_name / location_title_lat / location_title_lng / schedule_checkin_time，无 lat/lng/deviceid/sch_checkin_time）。
/// 官方权限表对「为打卡人员补卡」「添加打卡记录」「录入打卡人员人脸信息」标注第三方暂不支持，本接口不承载。
/// </para>
/// <para>企业自建应用见 <see cref="IWechatWorkInternalCheckinRecordService"/>；服务商代开发见 <see cref="IWechatWorkProviderCheckinRecordService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：第三方应用须具有「打卡」权限。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Checkin",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkCheckinRecordService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyCheckinRecordService : IWechatWorkCheckinRecordService
{
    /// <summary>
    /// 获取打卡记录数据
    /// <para>读取企业微信打卡软件打卡的原始数据（不包含企业微信 app 手机打卡的记录）。</para>
    /// <para>官方限制：获取打卡记录时间跨度与打卡类型有关——上下班打卡不能超过一个月、外出打卡不能超过三天、全部打卡不能超过三天（系统会自动校验，超过拒绝查询）；用户列表不超过 100 个，若超过请分批获取。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetCheckinDataRequest"/>：opencheckindatatype 打卡类型（第三方文档页标注为非必填） / starttime / endtime / useridlist 用户列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>打卡记录列表（checkindata：userid / groupname / checkin_type / exception_type / checkin_time / location_title / location_detail / wifiname / notes / wifimac / mediaids / agency_name / location_title_lat / location_title_lng / groupid / schedule_checkin_time）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94205"/></para>
    /// <para>官方权限：第三方应用须具有「打卡」权限；自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用须具有「打卡」权限。</para>
    /// <para>官方契约陷阱：本端点与自建/代开发文档页同路由不同构——checkin_type 参数表标注字符串但返回示例为整数（以示例的整数形态承载）；纬度/经度字段名为 location_title_lat/location_title_lng（区别于自建文档页的 lat/lng）。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/getcheckindata")]
    Task<GetThirdPartyCheckinDataResponse> GetCheckinDataAsync(
        [Body] GetCheckinDataRequest request,
        CancellationToken cancellationToken = default);
}
