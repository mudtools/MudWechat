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
/// 企业微信「打卡」模块打卡记录域企业自建应用 SDK。
/// <para>
/// 承载「获取打卡记录数据」（现代字段结构，代开发同构，见 <see cref="IWechatWorkProviderCheckinRecordService"/>）
/// 与官方仅自建开放的 3 个差异端点：「为打卡人员补卡」「添加打卡记录」「录入打卡人员人脸信息」
/// （官方权限表对代开发与第三方应用均标注暂不支持）。
/// </para>
/// <para>服务商代开发见 <see cref="IWechatWorkProviderCheckinRecordService"/>；第三方应用见 <see cref="IWechatWorkThirdPartyCheckinRecordService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：自建应用须配置到「打卡 - 可调用接口的应用」中。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Checkin",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkCheckinRecordService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalCheckinRecordService : IWechatWorkCheckinRecordService
{
    /// <summary>
    /// 获取打卡记录数据
    /// <para>获取可见范围内员工指定时间段内的打卡记录数据（有打卡记录即可获取，与当前「打卡应用」是否开启无关）。</para>
    /// <para>官方限制：获取记录时间跨度不超过 30 天；用户列表不超过 100 个，若超过 100 个请分批获取；标准打卡时间只对于固定排班和自定义排班两种类型有效；接口调用频率限制为 600 次/分钟。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetCheckinDataRequest"/>：opencheckindatatype 打卡类型 / starttime / endtime / useridlist 用户列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>打卡记录列表（checkindata：userid / groupname / checkin_type / exception_type / checkin_time / location_title / location_detail / wifiname / notes / wifimac / mediaids / lat / lng / deviceid / sch_checkin_time / groupid / schedule_id / timeline_id）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90262"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96497"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用与第三方应用须具有「打卡」权限。</para>
    /// <para>官方契约陷阱：checkin_type/exception_type 为中文字符串（如「上班打卡」「时间异常」），多个异常以分号间隔；lat/lng 为实际经纬度的 1000000 倍（GCJ-02）；第三方应用文档页为另一套旧字段结构（agency_name/location_title_lat/location_title_lng），由 <see cref="IWechatWorkThirdPartyCheckinRecordService"/> 承载。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/getcheckindata")]
    Task<GetCheckinDataResponse> GetCheckinDataAsync(
        [Body] GetCheckinDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 为打卡人员补卡
    /// <para>为指定成员补打卡记录（审批中的审批打卡不能补卡，补卡后原审批打卡的信息会清除）。</para>
    /// <para>官方限制：接口调用频率限制为 600 次/分钟；备注不超过 512 字节；无规则对应的打卡时间点（休息日打卡、无规则打卡、自由上下班）不传 schedule_checkin_time。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="PunchCorrectionRequest"/>：userid / schedule_date_time 应打卡日期 0 点 Unix 时间戳 / schedule_checkin_time 应打卡时间点（相对当天 0 点偏移秒） / checkin_time 实际打卡时间 Unix 时间戳 / remark 备注）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95803"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；官方权限表对代开发应用与第三方应用均标注暂不支持。</para>
    /// <para>官方契约陷阱：schedule_checkin_time 为相对当天 0 点的偏移秒数（如 32400 = 9:00），与 schedule_date_time/checkin_time 的 Unix 时间戳形态并存。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/punch_correction")]
    Task<WechatWorkResponse> PunchCorrectionAsync(
        [Body] PunchCorrectionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加打卡记录
    /// <para>通过接口写入打卡记录，匹配打卡规则后可在企业微信打卡明细、统计中参与展示。</para>
    /// <para>官方限制：一批最多 200 个记录；location_title/location_detail/notes/wifiname 限制 1024 字符、device_detail 限制 40 字符；mediaids 最多 1 个；wifimac 须满足六段冒号分隔正则且传入 wifiname 时必填；接口调用频率限制为 60 次/分钟；若原卡点为审批打卡，更新后原审批打卡的信息会清除。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddCheckinRecordRequest"/>：records 打卡记录列表（userid / checkin_time / location_title / location_detail / device_type / device_detail 等官方必填））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99647"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；官方权限表对代开发应用与第三方应用均标注暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/add_checkin_record")]
    Task<WechatWorkResponse> AddCheckinRecordAsync(
        [Body] AddCheckinRecordRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 录入打卡人员人脸信息
    /// <para>为企业打卡人员录入人脸信息（人脸信息仅用于人脸打卡；对已有人脸的用户会覆盖原有人脸，请谨慎操作）。</para>
    /// <para>官方限制：图片数据不超过 1M；接口调用频率限制为 10 次/分钟（打卡域最严格）；专属错误码 301021 输入参数错误 / 301069 输入 userid 无对应成员 / 301070 系统错误 / 301071 企业内有其他人员有相似人脸（此情况下人脸仍然会录入成功）/ 301072 人脸图像数据错误请更换图片。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="AddCheckinUserFaceRequest"/>：userid 用户 id / userface 人脸图片数据（base64 处理后填入，非 media_id））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93378"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；官方权限表对代开发应用与第三方应用均标注暂不支持。</para>
    /// <para>官方契约陷阱：userid 与 userface 的官方「必须」列标注为「否」（与常规必填预期不同，照抄）。</para>
    /// </remarks>
    [Post("/cgi-bin/checkin/addcheckinuserface")]
    Task<WechatWorkResponse> AddCheckinUserFaceAsync(
        [Body] AddCheckinUserFaceRequest request,
        CancellationToken cancellationToken = default);
}
