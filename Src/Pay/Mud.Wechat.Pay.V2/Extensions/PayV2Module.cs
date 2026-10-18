// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Extensions;

/// <summary>
/// 微信支付 APIv2 业务接口模块枚举（对齐 <c>PayModule</c>，三段式注册的第一段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>成员即 <c>RegistryGroupName</c>，且一律带 <c>V2</c> 后缀</b>：APIv3 线已用
/// <c>Transactions</c> / <c>Refund</c> / <c>Bill</c> 等裸名，两条线若同名会各产出一个同名扩展方法，
/// 跨程序集导入即歧义（同 <c>PayModule</c> 类注释里「不得叫 Pay」的理由）。
/// </para>
/// <para><b>成员只列已落地域</b>：每个成员必须在 <c>PayV2ServiceBuilder._registrars</c> 里有对应委托，
/// 否则落进「查不到注册器 ⇒ 静默不注册」分支，是无声陷阱。注册器分两种形态：带
/// <c>[HttpClientApi]</c> 的族挂生成的 <c>Add{组名}WebApiHttpClient()</c>（组名即成员名），
/// 需商户证书的手写族只挂 <c>TryAddSingleton</c>（<c>RedPackV2</c> 等族属后者，名单由
/// <c>WechatPayV2CertFamilyContractGuards.HandWrittenModules</c> 显式枚举，故<b>不</b>要求
/// 有同名生成注册器 —— 该不对称不靠「成员数 = 组数」的粗比对）。</para>
/// </remarks>
public enum PayV2Module
{
    /// <summary>
    /// APIv2 基础支付（13 端点）：统一下单 / 付款码支付 / 查单 / 关单 / 退款（含 v2）/
    /// 退款查询（含 v2）/ 撤销 / 下载对账单 / 下载资金账单 / 结算查询 / 汇率查询。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935214"/>。
    /// </summary>
    /// <remarks>
    /// 其中<b>需双向证书</b>的 5 支（<c>/secapi/pay/refund</c>、<c>/secapi/pay/refundv2</c>、
    /// <c>/secapi/pay/reverse</c>、<c>/pay/downloadbill</c>、<c>/pay/downloadfundflow</c>）
    /// <b>不在本组</b>：客户端证书按商户绑定，与「一条命名客户端一个连接池」的复用模型冲突，
    /// 由 <c>IWechatPayV2SecureService</c> 逐商户通道承载。
    /// </remarks>
    BasePayV2,

    /// <summary>
    /// APIv2 委托代扣（PAP）直连商户侧（6 端点）：支付中签约 / 扣款 / APP 预签约 /
    /// H5 预签约（本族唯一 GET）/ 查签约 / 解约。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987320"/>。
    /// </summary>
    /// <remarks>6 支<b>全部无需商户证书</b> ⇒ 整族走声明式生成，不属 <c>IWechatPayV2SecureService</c>。</remarks>
    PapayV2,

    /// <summary>
    /// APIv2 委托代扣（PAP）服务商侧（5 端点）：扣款 / APP 预签约 / H5 预签约 / 查签约 / 解约。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988372"/>。
    /// </summary>
    /// <remarks>
    /// 与 <see cref="PapayV2"/> 分属两个接口两个注册组：字段面多 <c>sub_appid</c>/<c>sub_mch_id</c>，
    /// 且<b>解约两支与商户侧共用同一路径</b>（官方文档即如此）⇒ 组名相同不会引起路由冲突，
    /// 但 DTO 不共用、模块必须分列，否则 <c>AddModules</c> 无法只装服务商侧。
    /// </remarks>
    PartnerPapayV2,

    /// <summary>
    /// APIv2 现金红包（5 端点，<b>全部需商户证书</b>）：发放红包 / 小程序红包 / 查询红包 /
    /// 发放企业红包（<c>workwx_sign</c> 双签）/ 查询企业红包。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011974053"/>。
    /// </summary>
    /// <remarks>
    /// <b>手写服务、无注册组</b>：证书按商户绑定 ⇒ 本成员在 <c>_registrars</c> 里只挂
    /// <c>TryAddSingleton</c>，<b>没有</b>对应的 <c>AddRedPackV2WebApiHttpClient()</c>
    /// （生成式注册器只属于带 <c>[HttpClientApi]</c> 的接口）。域名是 <c>api.mch</c> 而非 <c>api2.mch</c>。
    /// </remarks>
    RedPackV2,

    /// <summary>
    /// APIv2 企业付款到零钱（4 端点，<b>全部需商户证书</b>）：付款 / 查询 /
    /// 向员工付款（<c>workwx_sign</c> 双签，官方已标注暂不支持新增）/ 查询向员工付款。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011989673"/>。
    /// </summary>
    /// <remarks>
    /// 与 <see cref="RedPackV2"/> 同为手写族（证书 + <c>api.mch</c>）。
    /// 本族是<b>出资操作</b>，SDK 不自动重试：幂等键 <c>partner_trade_no</c> 换号即重复付款。
    /// </remarks>
    TransferV2,

    /// <summary>
    /// APIv2 服务商付款到银行卡（2 端点，<b>全部需商户证书</b>）：付款到银行卡 / 查询付款到银行卡。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011939840"/>。
    /// </summary>
    /// <remarks>
    /// 路径前缀 <c>/mmpaysptrans/</c>，与零钱族的 <c>/mmpaymkttransfers/</c> 不同。
    /// 卡号与户名官方要求 <b>RSA OAEP</b> 密文（公钥见 <c>/risk/getpublickey</c>），
    /// 查询支却要 <b>MD5</b> —— 两套口径由 SDK 之外的宿主加密面承担，本线不代做加密。
    /// </remarks>
    PartnerTransferV2,

    /// <summary>
    /// APIv2 受理押金（8 端点）：下单 / 人脸支付 / 付款码支付 / 查单 / 查退款（免证书，声明式）+
    /// 撤销 / 消费押金 / 退款（<b>需商户证书</b>，手写）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011986340"/>。
    /// </summary>
    /// <remarks>
    /// <b>一域两形态</b>：本成员同时挂 <c>AddDepositV2WebApiHttpClient()</c>（免证书五支）与
    /// <c>TryAddSingleton</c>（需证书三支）—— 证书按商户绑定，声明式管道拿不到，
    /// 但业务上同属押金域，故模块值只有一个、注册器内做两件事。域名恒为 <c>api.mch</c>。
    /// </remarks>
    DepositV2,

    /// <summary>
    /// APIv2 报关/订单附加信息（3 端点，均免证书）：提交 / 查询 / 重推。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985151"/>。
    /// </summary>
    /// <remarks>
    /// 路径前缀 <c>/cgi-bin/mch/customs/</c> 与 <c>/cgi-bin/mch/newcustoms/</c>（重推支）不同源，
    /// 本族三支报文<b>都没有</b> <c>nonce_str</c>（申报页请求侧还无 <c>sign_type</c>，
    /// 查询/重推两页的 <c>sign_type</c> 为选填）——
    /// 是 APIv2 线少见的例外形状，勿按通用形状补齐。
    /// </remarks>
    CustomsV2,

    /// <summary>
    /// APIv2 工具类（2 端点，均免证书）：付款码查询 openid / 转换短链接。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011940944"/>。
    /// </summary>
    ToolsV2,

    /// <summary>
    /// APIv2 风控（1 端点，<b>需商户证书</b>）：查询商户违规记录（机构 / 银行服务商侧）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/institution/4011985446"/>。
    /// </summary>
    /// <remarks>
    /// <b>手写服务、无注册组</b>（同 <see cref="RedPackV2"/> 口径）：证书按商户绑定 ⇒ 只挂
    /// <c>TryAddSingleton</c>。请求面<b>没有</b> <c>appid</c>，应答的记录是整段 JSON/CSV 字符串。
    /// </remarks>
    RiskV2,

    /// <summary>
    /// APIv2 服务商子商户配置（3 端点，<b>全部需商户证书</b>）：新增支付目录 / 查询配置 / 新增关注配置。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115404"/>。
    /// </summary>
    /// <remarks>
    /// <b>手写服务、无注册组</b>：三支官方「是否需要证书」栏均为「是」。
    /// 域名仍是 <c>api.mch</c> 而非 <c>api2.mch</c> —— <c>/secapi/</c> 前缀只说明「要证书」。
    /// 两支支付目录（<c>/secapi/mch/*</c>）报文<b>没有</b> <c>nonce_str</c> 与 <c>sign_type</c>，
    /// 关注配置（路径段是 <c>mkt</c> 不是 <c>mch</c>：<c>/secapi/mkt/addrecommendconf</c>）
    /// 则两者必填且仅 HMAC-SHA256 ⇒ 同域三形态，勿抽公共请求基类。
    /// </remarks>
    SubMerchantConfigV2,

    /// <summary>
    /// APIv2 小微商户资料修改（2 端点，<b>全部需商户证书</b>）：结算银行卡 / 联系人信息。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115349"/>。
    /// </summary>
    /// <remarks>
    /// 路径 <c>/applyment/micro/*</c>，域名 <c>api.mch</c>。<c>sign_type</c> 官方「仅支持 HMAC-SHA256」
    /// 且该字段参与 <c>sign</c> 计算（默认值即契约面）。
    /// <b>加密面在本模块之外</b>：<c>account_number</c>／<c>mobile_phone</c>／<c>email</c> 是敏感字段，
    /// 须由宿主用平台证书公钥加密后上传（SDK 侧入口 <c>WechatPayV2SensitiveFieldEncrypter</c>），
    /// <c>cert_sn</c> 必须是加密所用那张平台证书的序列号。
    /// </remarks>
    MerchantMicroV2,

    /// <summary>
    /// APIv2 境外机构子商户进件（3 端点，<b>全部需商户证书</b>）：新增 / 修改 / 查询。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013636642"/>。
    /// </summary>
    /// <remarks>
    /// <b>本族是本线唯一挂 <c>apihk.mch</c> 的业务域</b>（三页「接口说明」栏只印 <c>apihk</c>，
    /// 全文不出现 <c>api.mch</c>）⇒ 通道域名取 <c>WechatPayV2HttpClientNames.GlobalBaseAddress</c>。
    /// 主体标识字段是 <c>app_id</c>（带下划线，与其余各族的 <c>appid</c> 不同）、
    /// 签名<b>仅 MD5</b>（官方「默认加密方式为MD5，且仅支持MD5加密」）⇒ 报文无 <c>sign_type</c>。
    /// </remarks>
    InstitutionSubV2,

    /// <summary>
    /// APIv2 商户媒体上传（1 端点，<b>需商户证书</b>）：上传图片换取 <c>media_id</c>。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013641477"/>。
    /// </summary>
    /// <remarks>
    /// <b>本线唯一的 multipart/form-data 端点</b>（请求表单、应答仍是 XML）⇒ 必须手写；
    /// 文件分节 <c>media</c> 不参与签名，完整性由 <c>media_hash</c>（<b>小写</b> MD5）承载。
    /// 域名与 <see cref="InstitutionSubV2"/> 同为 <c>apihk.mch</c>；产物 <c>media_id</c> 供机构进件引用。
    /// </remarks>
    MerchantMediaV2,

    /// <summary>
    /// APIv2 车主平台·直连商户（3 端点，均免证书）：下单通知 / 支付申请 / 查询支付状态。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985272"/>。
    /// </summary>
    /// <remarks>
    /// 路径前缀 <c>/vehicle/pay/</c>，域名 <c>api.mch</c>。<c>sign_type</c> 官方默认即 HMAC-SHA256
    /// ⇒ 默认值写进 DTO，是算法开关也是契约面。三支均为「异步受理」语义：
    /// 下单通知与支付申请的 <c>SUCCESS</c> 只代表受理，真实扣款结果只能靠轮询 <c>querystate</c>；
    /// <c>payapply</c> 失败须按官方给定的十级退避序列重试。
    /// </remarks>
    VehicleV2,

    /// <summary>
    /// APIv2 车主平台·服务商（3 端点，均免证书）：下单通知 / 支付申请 / 查询支付状态。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988204"/>。
    /// </summary>
    /// <remarks>
    /// 与 <see cref="VehicleV2"/> 分属两个接口两个注册组：本族 <c>sub_appid</c> 与 <c>sub_mch_id</c>
    /// <b>同为必填</b>，路径多一段 <c>partner</c>（<c>/vehicle/partnerpay/*</c>），
    /// 且 <c>trade_scene</c> 取值集比直连侧少 <c>BRIDGE</c> ⇒ DTO 不共用、模块必须分列。
    /// </remarks>
    PartnerVehicleV2,

    /// <summary>
    /// APIv2 乘车码·直连商户（1 端点，免证书）：代扣订单查询。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011986341"/>。
    /// </summary>
    /// <remarks>
    /// <b>灰度测试接口</b>（官方原文警示），且只查 <c>trade_type=PAP</c> 的代扣订单 ⇒
    /// <c>trade_state</c> 只有三档（无 <c>REFUND</c>）。应答含 <c>coupon_type_0/1/…</c> 下标字段，
    /// 由 DTO 的 <c>IndexedFields</c> 承接（条数取 <c>coupon_count</c>）。
    /// </remarks>
    TransitV2,

    /// <summary>
    /// APIv2 乘车码·服务商（1 端点，免证书）：代扣订单查询。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011989909"/>。
    /// </summary>
    /// <remarks>
    /// <b>接口版本号参与报文</b>：<c>version</c> 传 <c>2.0</c> 才可能拿到 <c>bank_type=BPA</c>（垫资）
    /// 与 <c>user_repaid</c>，且 <c>trade_state</c> 比直连侧多一档 <c>REFUND</c> ⇒ 应答 DTO 不共用。
    /// </remarks>
    PartnerTransitV2,

    /// <summary>
    /// APIv2 支付验收·交易信息上报（1 端点，免证书）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935222"/>。
    /// </summary>
    /// <remarks>
    /// 路径 <c>/payitil/report</c>，域名 <c>api.mch</c>。本支是<b>旁路遥测</b>：
    /// 应答无 <c>sign</c>（不可验签）、官方错误码栏写「无」⇒ 失败不得阻塞主链路。
    /// 请求字段 <c>execute_time_</c> 末尾带下划线，官方原文注明「参数设计如此，非文档问题」⇒ 照抄不得纠正。
    /// </remarks>
    PayItilV2,

    /// <summary>
    /// APIv2 沙箱·取 <c>sandbox_signkey</c>（1 端点，免证书）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011984810"/>（支付验收指引内页）。
    /// </summary>
    /// <remarks>
    /// 路径 <c>/xdc/apiv2getsignkey/sign/getsignkey</c>，域名 <c>api.mch</c>。
    /// 请求面<b>只有</b> <c>mch_id</c>／<c>nonce_str</c>／<c>sign</c>（无 <c>appid</c>、无 <c>sign_type</c>
    /// ⇒ 按 MD5 缺省档签名），应答无 <c>sign</c> 也无 <c>result_code</c>。
    /// <b>产物是凭据</b>：<c>sandbox_signkey</c> 不得写日志或长期缓存。
    /// </remarks>
    XdcV2,

    /// <summary>
    /// APIv2 订单评价拉取（1 端点，<b>需商户证书</b>）：按评论时间批量拉取。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4014326528"/>。
    /// </summary>
    /// <remarks>
    /// <b>手写服务、无注册组</b>：官方「是否需要证书」为「需要」，且证书缺失时的
    /// <c>return_msg</c> 是「商户证书错误」。域名是 <c>api.mch</c> 而非 <c>api2.mch</c>。
    /// <b>应答不是 XML</b>：成功报文是逐行以反引号开头、逗号分隔的文本表，首行为下一批的
    /// <c>offset</c> 游标 ⇒ 本成员挂的服务返回 <see cref="string"/> 原文，失败 XML 信封原样交回由调用方判别。
    /// </remarks>
    BillCommentV2,

    /// <summary>
    /// APIv2 防欺诈·取平台公钥（1 端点，<b>需商户证书</b>）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011941097"/>。
    /// </summary>
    /// <remarks>
    /// <b>本支是本线唯一挂在 <c>fraud.mch</c> 域名上的端点</b>（第四个闭集域名，
    /// 见 <c>WechatPayV2HttpClientNames.FraudBaseAddress</c>）⇒ 手写族 + (商户, 域名) 双键通道。
    /// 产物 <c>pub_key</c> 是 RSA <b>公钥</b>（非机密），但它是付款到银行卡
    /// <c>enc_bank_no</c>／<c>enc_true_name</c> 的加密前置；应答<b>无 <c>sign</c></b> ⇒ 不可验签。
    /// </remarks>
    FraudV2,

    /// <summary>
    /// APIv2 资金·按日期查询自动提现（1 端点，<b>需商户证书</b>，服务商侧小微商户）。
    /// 官方文档 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115346"/>。
    /// </summary>
    /// <remarks>
    /// <b>手写服务、无注册组</b>：官方「是否需要证书」为「需要」，错误码表另有 <c>CA_ERROR</c>。
    /// 域名仍是 <c>api.mch</c>。请求面无 <c>appid</c>、<c>mch_id</c> 与 <c>sub_mch_id</c> 同为必填，
    /// 且<b>提现单日期比交易日晚一天</b>（传错得到「查无此单」而不是报错）。
    /// 应答<b>带 <c>sign</c></b> ⇒ 走常规验签（与 <see cref="FraudV2"/> 同出口不同报文事实）。
    /// </remarks>
    FundV2,
}
