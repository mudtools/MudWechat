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
/// 城市服务·适老化医疗域（2 端点：医保实名信息获取 + 消息订阅关系查询；
/// 路由族 <c>/cityservice/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>个人敏感信息红线</b>：<c>open_id</c>、<c>wxmed_authcode</c>、密文姓名与其解密后明文
/// 一律禁入日志、遥测与异常消息。
/// </para>
/// <para>
/// <b>官方文档树两处</b>：<c>getmedrealname</c> 现位于
/// <c>miniprogram/dev/server/API/cityservice/elderMedical/</c>；<c>getmsgrelation</c> 同族。
/// 旧 <c>doc/offiaccount/elderMedical/elderMedical-api.html</c> 已 301 到
/// <c>doc/service/guide/product/</c> 概览页，故出处注释以现行服务端 API 页为准。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "CityService", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaCityServiceElderMedicalService
{
    /// <summary>
    /// 获取医保实名信息（<c>POST /cityservice/getmedrealname</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cityservice/elderMedical/api_cityservice_getmedrealname"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCityServiceGetMedRealNameRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>密文实名信息与密钥版本，见 <see cref="WxaCityServiceGetMedRealNameResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b><c>wxmed_authcode</c> 有效期 10 分钟</b>：由医保侧业务页面下发，过期即 11200
    /// 「code 无效」；SDK 不缓存、不重放该凭据。
    /// </para>
    /// <para>
    /// <b>返回的是密文</b>：<c>cipher_real_name</c> 须用与本接口 <c>key_version</c> 对应的密钥、
    /// 按 <c>cipher_algorithm</c>（当前为 <c>AES_256_ECB_PKCS7Padding</c>）解密；SDK 不持有密钥，
    /// 不做本地解密。
    /// </para>
    /// <para>
    /// <b>官方回包示例字段名不一致（诚实留档）</b>：参数表写 <c>open_id</c>，示例写作 <c>openid_id</c>
    /// ⇒ 本 SDK 按参数表建模，不以示例覆盖字段表。
    /// </para>
    /// </remarks>
    [Post("/cityservice/getmedrealname")]
    Task<WxaCityServiceGetMedRealNameResponse> GetMedRealNameAsync(
        [Body] WxaCityServiceGetMedRealNameRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询消息订阅关系（<c>POST /cityservice/getmsgrelation</c>，查询类端点官方仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/cityservice/elderMedical/api_cityservice_getmsgrelation"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="WxaCityServiceGetMsgRelationRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订阅状态（<c>1</c> 已订阅 / <c>0</c> 未订阅），见 <see cref="WxaCityServiceGetMsgRelationResponse"/>。</returns>
    /// <remarks>
    /// <para><b><c>business_id</c> 取官方业务编号</b>：适老化医疗域为 <c>130</c>，传其它值语义未定义。</para>
    /// <para>
    /// <b>官方回包示例用 <c>err_code</c>/<c>err_msg</c></b>（与全站 <c>errcode</c>/<c>errmsg</c> 不同）；
    /// 本 SDK 按统一错误信封建模，示例差异留档。
    /// </para>
    /// </remarks>
    [Post("/cityservice/getmsgrelation")]
    Task<WxaCityServiceGetMsgRelationResponse> GetMsgRelationAsync(
        [Body] WxaCityServiceGetMsgRelationRequest request,
        CancellationToken cancellationToken = default);
}
