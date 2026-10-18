// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.CityService;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 城市服务·基础能力域（3 端点：服务首页路径获取 + 渠道消息下发 + 消息数据下发；
/// 路由族 <c>/cityservice/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>个人敏感信息红线</b>：<c>open_id</c>、<c>order_id</c>、<c>msg_id</c>、就诊人姓名与医院业务信息
/// 一律禁入日志、遥测与异常消息。
/// </para>
/// <para>
/// <b>官方文档树两处</b>：<c>getservicepath</c> 与 <c>sendmsgdata</c> 位于
/// <c>miniprogram/dev/server/API/cityservice/basic/</c>；<c>sendchannelmsg</c> 现位于
/// <c>doc/service/api/medicalassistant/</c>。旧 <c>platform-capabilities/cityservice/*</c> 页面
/// 仅为跳转壳，故出处注释取现行 API 页。
/// </para>
/// <para>
/// <b>业务准入</b>：本域面向公立医院与卫健委等城市服务合作方，调用前须完成行业资质与场景报备；
/// 普通小程序调用将返回权限类错误。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "CityService", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaCityServiceBasicService
{
    /// <summary>
    /// 获取城市服务首页路径（<c>POST /cityservice/getservicepath</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cityservice/basic/api_cityserviceservicehomepath"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCityServiceGetServicePathRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>跳转路径与业务标识，见 <see cref="WxaCityServiceGetServicePathResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>条件必填</b>：<c>page_type=1</c>（首页）须 <c>city_name</c>；<c>page_type=3</c>（专题页）
    /// 须 <c>content_name</c>；<c>page_type=5</c>（服务列表）须 <c>service_id</c> 或 <c>params</c>；
    /// <c>page_type=0</c>（服务首页）不附加定位条件。
    /// </para>
    /// <para>
    /// <b>官方示例含格式缺陷（诚实留档）</b>：回包示例出现单引号、尾部多余引号，且响应键拼作
    /// <c>bussiness_type</c>；本 SDK 按参数表的 <c>business_type</c> 建模。
    /// </para>
    /// <para>错误码 <c>83200</c> 表示城市服务能力未开通或参数不满足场景要求。</para>
    /// </remarks>
    [Post("/cityservice/getservicepath")]
    Task<WxaCityServiceGetServicePathResponse> GetServicePathAsync(
        [Body] WxaCityServiceGetServicePathRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下发渠道消息（<c>POST /cityservice/sendchannelmsg</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/medicalassistant/api_cityservice_sendchannelmsg"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCityServiceSendChannelMsgRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>统一错误信封，见 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>仅公立医院 / 卫健委可调用</b>：官方限定医疗场景合作方，<c>business_id</c> 取 <c>150</c>；
    /// 其它主体调用返回权限错误。
    /// </para>
    /// <para>
    /// <b><c>status</c> 为业务状态</b>（官方未发布完整枚举），SDK 只建模 <c>long</c>，不臆造常量。
    /// </para>
    /// <para>
    /// <b>跳转页两型</b>：<c>redirect_page.page_type</c> 取 <c>web</c> / <c>mini_program</c>
    /// （见 <see cref="WxaCityServiceRedirectPageTypes"/>），后者须同时给 <c>app_id</c> 与 <c>full_path</c>。
    /// </para>
    /// </remarks>
    [Post("/cityservice/sendchannelmsg")]
    Task<WxaResponse> SendChannelMsgAsync(
        [Body] WxaCityServiceSendChannelMsgRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 下发消息数据（<c>POST /cityservice/sendmsgdata</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cityservice/basic/api_cityservice_sendmsgdata"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCityServiceSendMsgDataRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>结果页地址，见 <see cref="WxaCityServiceSendMsgDataResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b><c>data</c> 为模板变量对象</b>：结构随 <c>biz_template_id</c> 变化，SDK 以
    /// <see cref="System.Text.Json.JsonElement"/> 透传，不做本地结构校验。
    /// </para>
    /// <para>
    /// <b>错误码族成组</b>：<c>40097</c> 参数错误、<c>48001</c> 接口未授权、
    /// <c>82020</c>~<c>82026</c> 模板 / 样式 / 订单号相关业务拒绝，逐码见官方错误表。
    /// </para>
    /// </remarks>
    [Post("/cityservice/sendmsgdata")]
    Task<WxaCityServiceSendMsgDataResponse> SendMsgDataAsync(
        [Body] WxaCityServiceSendMsgDataRequest request,
        CancellationToken cancellationToken = default);
}
