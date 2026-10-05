// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Checkin;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「打卡」模块打卡记录域服务商代开发 SDK。
/// <para>
/// 承载「获取打卡记录数据」（现代字段结构，与自建应用文档页同构，见 <see cref="IWechatWorkInternalCheckinRecordService"/>）；
/// 官方权限表对「为打卡人员补卡」「添加打卡记录」「录入打卡人员人脸信息」标注代开发暂不支持，本接口不承载。
/// </para>
/// <para>企业自建应用见 <see cref="IWechatWorkInternalCheckinRecordService"/>；第三方应用见 <see cref="IWechatWorkThirdPartyCheckinRecordService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费代开发授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：代开发应用须具有「打卡」权限。
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
public interface IWechatWorkProviderCheckinRecordService : IWechatWorkCheckinRecordService
{
    /// <summary>
    /// 获取打卡记录数据
    /// <para>获取可见范围内员工指定时间段内的打卡记录数据（有打卡记录即可获取，与当前「打卡应用」是否开启无关）。</para>
    /// <para>官方限制：获取记录时间跨度不超过 30 天；用户列表不超过 100 个，若超过 100 个请分批获取；接口调用频率限制为 600 次/分钟。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetCheckinDataRequest"/>：opencheckindatatype 打卡类型 / starttime / endtime / useridlist 用户列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>打卡记录列表（checkindata：userid / groupname / checkin_type / exception_type / checkin_time / location_title / location_detail / wifiname / notes / wifimac / mediaids / lat / lng / deviceid / sch_checkin_time / groupid / schedule_id / timeline_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90262"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96497"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用与第三方应用须具有「打卡」权限。</para>
    /// <para>官方契约陷阱：checkin_type/exception_type 为中文字符串（如「上班打卡」「时间异常」），多个异常以分号间隔；lat/lng 为实际经纬度的 1000000 倍（GCJ-02）。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/getcheckindata")]
    Task<GetCheckinDataResponse> GetCheckinDataAsync(
        [Body] GetCheckinDataRequest request,
        CancellationToken cancellationToken = default);
}
