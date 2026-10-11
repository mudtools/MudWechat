// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Extensions;

/// <summary>
/// 微信公众号 API 模块枚举（对齐企微 <c>WechatModule</c>）。
/// </summary>
/// <remarks>
/// 令牌签发客户端（<c>IMpAuthentication</c>）位于 Abstractions 的 Authentication 注册组，
/// 随令牌底座自动注册，不经本枚举。
/// </remarks>
public enum MpModule
{
    /// <summary>
    /// 基础接口（获取微信 API 服务器 IP + 获取微信推送服务器 IP + 网络通信检测 3 端点）。
    /// </summary>
    /// <remarks>
    /// 公众号无「自建 / 套件 / 代开发」三类形态 ⇒ 单一接口直接作注册接口（不设应用类型子接口）。
    /// 官方对 3 个端点均支持第三方平台令牌（<c>component_access_token</c> /
    /// <c>authorizer_access_token</c>），<b>M0 仅覆盖自建形态</b>（AppId + AppSecret）。
    /// </remarks>
    Basic,

    /// <summary>
    /// 用户管理 → 标签管理（创建标签 / 获取标签 / 编辑标签 / 删除标签 / 获取标签下粉丝列表 /
    /// 批量打标签 / 批量取消标签 / 获取用户标签列表共 8 端点）。
    /// </summary>
    /// <remarks>
    /// 域级约束：全部端点适用范围均为「公众号 / 服务号 —— 仅认证」（仅企业主体已认证账号可调用）；
    /// 标签上限 100 个、单用户标签上限 20 个、标签名 ≤ 30 字符。
    /// </remarks>
    Tag,

    /// <summary>
    /// 用户管理 → 用户信息（获取用户基本信息 / 批量获取用户基本信息 / 获取关注用户列表 / 设置用户备注名 /
    /// 获取黑名单列表 / 拉黑用户 / 取消拉黑用户共 7 端点）。
    /// </summary>
    /// <remarks>
    /// 域级约束：均「仅认证」；<c>updateRemark</c> 正文原文为「暂时开放给微信认证的服务号」；
    /// 批量 100 条 / 关注者单批 10000 / 黑名单单批 1000 / 拉黑单次 20 个。
    /// </remarks>
    User,

    /// <summary>
    /// 自定义菜单（创建菜单 / 获取菜单 / 删除菜单 / 创建个性化菜单 / 删除个性化菜单 / 测试个性化菜单匹配 /
    /// 查询自定义菜单信息共 7 端点）。
    /// </summary>
    /// <remarks>
    /// 域级约束：菜单规模硬上限（3 个一级 / 每级 5 个二级）；个性化菜单每日新增 2000、删除 2000、测试 20000；
    /// 删除默认菜单会级联删除全部个性化菜单；无个性化菜单编辑 API。
    /// </remarks>
    Menu,

    /// <summary>
    /// 客服消息 → 客服消息（发送客服消息 / 客服输入状态 / 获取聊天记录共 3 端点）。
    /// </summary>
    /// <remarks>
    /// 域级约束：下发额度 5 条/48 小时（用户发消息）或 3 条/1 分钟（菜单、关注、扫码）；
    /// 输入状态需 30 秒内有过交互；聊天记录查询区间 ≤ 24 小时、每次 ≤ 10000 条。
    /// 官方适用范围为「公众号 / 服务号 均仅认证」（**非服务号专属**），故不设账号类型本地闸。
    /// </remarks>
    CustomerMessage,

    /// <summary>
    /// 客服消息 → 客服管理（获取全部 / 在线客服列表、增删改客服账号、设置客服头像、邀请绑定共 7 端点）。
    /// </summary>
    /// <remarks>
    /// 域级约束：每账号最多 100 个客服账号；账号形状「前缀(≤10，英文/数字/下划线)@公众号微信号(≤30)」；
    /// 公众号须先在官网设置微信号；未绑定微信号的客服账号不能投入使用（须走邀请绑定）。
    /// </remarks>
    KfAccount,

    /// <summary>
    /// 客服消息 → 会话控制（创建 / 关闭会话、会话状态 / 列表、未接入列表共 5 端点）。
    /// </summary>
    /// <remarks>
    /// 域级约束：创建会话要求客服已绑定微信号<b>且在线</b>；未接入列表最多返回 100 条且无分页游标。
    /// </remarks>
    KfSession,

    /// <summary>
    /// 模板消息（发送模板消息 / 设置行业 / 获取行业 / 选用模板 / 获取已选用列表 / 删除模板 /
    /// 查询拦截的模板消息共 7 端点，<b>服务号专属</b>）。
    /// </summary>
    /// <remarks>
    /// 域级约束：7 页适用范围均为「服务号（仅认证）」（SDK 不做本地闸，官方 48001 表达）；
    /// 频率上限为指南页「日调用上限 10 万次」；行业每月可修改 1 次（修改后原行业模板删除）；
    /// 每账号可同时使用 25 个模板；发送结果经回调 templatesendjobfinish 异步回执。
    /// 文档位于服务号域 /doc/service/api/（非订阅号域）。
    /// </remarks>
    Template,

    /// <summary>
    /// 订阅通知（发送订阅通知 bizsend 1 + 模板管理 /wxaapi/newtmpl/* 6，共 7 端点，<b>服务号专属</b>）。
    /// </summary>
    /// <remarks>
    /// 域级约束：bizsend 页适用范围「公众号 / 服务号 —— 仅认证」，newtmpl 六页「小程序 ✔ / 公众号 仅认证 /
    /// 服务号 仅认证 / 小游戏 ✔」（小程序文案复用痕迹，照录）；一次性消耗用户订阅次数；
    /// 发送结果经回调 subscribe_msg_sent_event 异步回执；模板管理路径前缀 /wxaapi/newtmpl/（无 /cgi-bin 段）。
    /// </remarks>
    SubscriptionNotice,

    /// <summary>
    /// openApi 管理（重置 API 调用次数 / AppSecret 重置 / 额度查询 / 指定 API 清零 / rid 查询共 5 端点，
    /// 双接口同注册组：IMpOpenApiService 4 端点带令牌 + IMpOpenApiTokenFreeService 1 端点免令牌）。
    /// </summary>
    /// <remarks>
    /// 域级约束：clear_quota 与 clear_quota/v2 合计每月 10 次清零；openapi/quota/clear 每月 50 次；
    /// rid 查询仅同账号且有效期 7 天；clear_quota/v2 为「access_token 耗尽」应急逃生端点（I4 裁决独立接口）。
    /// </remarks>
    OpenApi,

    /// <summary>
    /// 网页授权（sns 四端点，<b>服务号专属</b>；全部<b>免令牌端点</b>——不消费应用级 access_token）。
    /// </summary>
    /// <remarks>
    /// 域级约束：4 页适用范围「服务号 —— 仅认证」；频率限制 5 万/分钟（access_token/refresh_token/userinfo 三页）；
    /// I4/I5 裁决：无 [Token] 特性、不建模用户级令牌管理器，refresh_token（30 天）生命周期归宿主；
    /// sns/oauth2/access_token 的 secret 走 Query（官方契约，脱敏词表已覆盖）。
    /// </remarks>
    Sns,

    /// <summary>
    /// 群发消息（sendall/send/preview/delete/get/speed 双端点共 7 端点；uploadimg 归素材域、
    /// uploadnews 官方标注迁移草稿箱不建模——N5 裁决）。
    /// </summary>
    /// <remarks>
    /// 域级约束：mass/send 服务号专属（官方原文，48001 表达）；提交成功 ≠ 群发完成，
    /// 结果经回调 masssendjobfinish 异步推送；clientmsgid 24 小时防重；
    /// 核验页面无群发频次上限数值（45028 由官方表达），SDK 不编造数值。
    /// </remarks>
    Mass,

    /// <summary>
    /// 服务号二维码·带参二维码（qrcode/create 1 端点，<b>服务号专属</b>；
    /// showqrcode 换图走 mp.weixin.qq.com 域名、官方无须登录态，SDK 暂不建模——XML 记录换图方式）。
    /// </summary>
    Qrcode,

    /// <summary>
    /// 自动回复（get_current_autoreply_info 1 端点，只读查询；公众号 / 服务号均可——
    /// 官方原文「认证/未认证的服务号/订阅号，以及接口测试号，均拥有该接口权限」）。
    /// </summary>
    AutoReply,

    /// <summary>
    /// 草稿管理（draft/add / update / get / delete / count / batchget 共 6 端点；draft/switch 官方已废弃不实现）。
    /// </summary>
    /// <remarks>
    /// 域级约束：草稿被群发或发布后从草稿箱移除；add 的 articles 为数组、update 的为单对象（官方两页不一致照录）；
    /// batchget count 1~20；「单篇 8 条上限」在官方页面无原文（仅第三方转述，不编造）。
    /// </remarks>
    Draft,

    /// <summary>
    /// 发布能力（freepublish/submit / get / delete / getarticle / batchget 共 5 端点，仅认证）。
    /// </summary>
    /// <remarks>
    /// 域级约束：提交成功不等于发布完成（结果经 PUBLISHJOBFINISH 事件推送——官方无独立 XML 页，事件暂不建模）；
    /// publish_status 标量 0~6；batchget count 1~20（官方原文，非旧版口径 100）；条目键 article_id（非 item_id）。
    /// </remarks>
    FreePublish,

    /// <summary>
    /// 商品卡片（channels/ec/service/product/getcardinfo 1 端点；/channels/ec/ 视频号小店域前缀，非 /cgi-bin/）。
    /// </summary>
    ProductCard,

    /// <summary>
    /// 留言管理（comment/open / close / list / markelect / unmarkelect / delete / reply/add / reply/delete 共 8 端点，仅认证 + 留言权限）。
    /// </summary>
    /// <remarks>
    /// 域级约束：需留言功能权限（88000）；均以 msg_data_id + index 定位文章（非 article_id）；
    /// 评论列表 count 50 以内（88010）。
    /// </remarks>
    Comment,

    /// <summary>
    /// 数据统计（21 端点单域承载：用户 2 + 图文 10 + 消息 7 + 接口 2；全部 POST /datacube/*、请求体同构——N3 裁决不拆 4 域）。
    /// </summary>
    /// <remarks>
    /// 域级约束：适用范围全部「公众号 / 服务号 —— 仅认证」；跨度上限措辞逐端点核验（7 天 / 15 天 / 30 天 / 1 天 /
    /// 必须为同一天——详见 MpDateRangeRequest remarks）；旧图文 6 端点官方声明已停止维护（照常建模并标注）；
    /// 越界由官方 61501 表达，SDK 不做本地校验。
    /// </remarks>
    DataCube,

    /// <summary>
    /// 令牌签发（<c>getAccessToken</c> + <c>getStableAccessToken</c>）：随 <c>AddMpApp</c> 自动注册，
    /// 本枚举成员仅供模块清单对齐使用（不作为 <c>AddAuthenticationApi()</c> 的必要入口）。
    /// </summary>
    Authentication,

    /// <summary>
    /// 素材管理（临时素材上传 1 端点 + 下载通道 2 端点；永久素材与 uploadimg 随 P0-b 批次并入本域）。
    /// </summary>
    /// <remarks>
    /// 域级约束：临时素材 3 天有效且可复用；类型大小限制逐类核验（image 10M / voice 2M·60s /
    /// video 10M / thumb 64KB；官方页面内两处表述不一致，以字段表为口径，详见接口 remarks）。
    /// <b>双通道</b>：上传（JSON 响应）走生成管线；下载（<c>media/get</c> / <c>media/get/jssdk</c>，
    /// 文件流响应）走 <c>IMpMediaDownloadService</c> 独立请求形态（Content-Type 分支判错）。
    /// </remarks>
    Media,

    /// <summary>
    /// 智能接口（12 端点单域承载：AI 开放接口 3 + OCR 识别 7 + 图像处理 2——
    /// 三种官方路径前缀 <c>/cgi-bin/media/voice/*</c>、<c>/cv/ocr/*</c>、<c>/cv/img/*</c> 照实同域）。
    /// </summary>
    /// <remarks>
    /// 域级约束：账号适用范围 OCR / 图像处理九端点为「公众号 / 服务号 —— <b>仅认证</b>」、
    /// AI 三端点为全开放；OCR 七端点 100 次/天（<b>菜单识别页无频率上限</b>），
    /// 图像处理与 AI 页无频率数值；图片小于 2M；OCR / 图像处理九端点支持第三方平台代调用（权限集 117）。
    /// <b>不做编排</b>：AI 语音「上传 → 10s 内轮询」两步语义只写 XML。
    /// <b>9 端点双调用形态</b>（form 上传 <c>img</c> / Query <c>img_url</c> 互斥）⇒ 每端点双方法
    /// （生成管线对 <c>[MultipartForm]</c> 参数无 null 分支，单方法可选形态会 NRE）。
    /// </remarks>
    SmartApi,

    /// <summary>
    /// 扫二维码打开小程序（<c>/cgi-bin/wxopen/qrcodejump*</c> 4 端点，<b>服务号专属</b>）。
    /// </summary>
    /// <remarks>
    /// 域级约束：官方 5 次/秒（错误码 44990）；发布配额每月 100 次（错误码 886000，
    /// 可用 <c>qrcodejumpget</c> 的 <c>qrcodejump_pub_quota</c> 前置探量）；须先关联小程序（否则 61007）；
    /// 支持第三方平台代调用（权限集 3、18）。
    /// 与 <see cref="Qrcode"/> 域的分工：本域管理小程序跳转<b>规则</b>，非带参二维码 ticket。
    /// </remarks>
    QrcodeJump,

    /// <summary>
    /// 长信息与短链（<c>/cgi-bin/shorten/*</c> 2 端点）。
    /// </summary>
    /// <remarks>
    /// 域级约束：<c>long_data</c> ≤ 4KB、<c>expire_seconds</c> ≤ 2592000 秒（30 天，默认同值）——
    /// 越界由官方 9410010/9410011 表达，SDK 不做本地拦截；
    /// 适用范围「小程序 ✔ / 服务号 仅认证 / 小游戏 ✔」；官方无频率数值；
    /// 支持第三方平台代调用（权限集 3、17）。
    /// </remarks>
    ShortLink,

    /// <summary>
    /// 微信门店 → 门店小程序（12 端点：类目 / 主体申请与审核 / 修改主体 / 省市区 / 地图点位搜索 /
    /// 门店增查列删改 / 地图建店）。
    /// </summary>
    /// <remarks>
    /// 域级约束（逐页核验，勿弱化）：<b>行业资质门槛极窄</b>——官方错误码 <c>43104</c> 原文「仅开放给
    /// 电商类目（电商平台、商家自营、跨境电商）」；主体级配额上限（管理员手机 / 微信号 / 身份证 / 主体各 5 次，
    /// 头像或简介月修改上限）；无新票据体系（<c>card_id</c> 仅透传卡券 id，不消费卡券 <c>api_ticket</c>）；
    /// 与支付体系不耦合；12 页全部支持第三方平台代调用（权限集 8-10、13 / 8-10、13、37）；
    /// 适用范围页间不一致（4 页含小程序，8 页仅公众号 / 服务号），全部无「仅认证」标注；无频率数值。
    /// <b>官方文档冲突三段式处置</b>（存在性取并集 / 标量类型取示例 / 容器形态取表或示例）见
    /// <c>IMpStoreService</c> remarks。
    /// </remarks>
    Store,

    /// <summary>
    /// 微信「一物一码」（<c>/intp/marketcode/*</c> 6 端点：申请 / 查询申请单 / 下载二维码包 / 激活 /
    /// 查询激活状态 / CODE_TICKET 换 CODE）。
    /// </summary>
    /// <remarks>
    /// 域级约束（逐页核验，勿弱化）：账号门槛为「<b>服务号（需申请）</b>」（须先申请开通，非「仅认证」）；
    /// 无新票据体系（下载所得 <c>buffer</c> 走 base64 decode + 解密，与令牌无关）；与支付不耦合；
    /// 6 页全部支持第三方平台代调用（权限集 <b>46</b>）；无频率数值；
    /// <b>响应为平级字段</b>（无 <c>data</c> 包裹）；<b>错误码面极薄</b>（仅通用 <c>40001</c>）⇒ 本域不新增错误码常量；
    /// <c>isv_application_id</c> 为幂等键；<c>code_count</c> 须 10000 的整数倍且 ∈ [10000, 20000000]。
    /// </remarks>
    OneCode,

    /// <summary>
    /// 微信发票（17 端点单域承载：商户开票 5 + 开票平台 5 + 发票报销 4 + 极速开发票 3）。
    /// </summary>
    /// <remarks>
    /// 域级约束（逐页核验，勿弱化）：<b>17 页全部消费应用级 <c>access_token</c>，不引入 <c>api_ticket</c> 建模</b>
    /// （<c>getauthurl</c> 的 <c>ticket</c> 为授权页票据、<c>s_pappid</c> 为开票平台标识，均非票据）；
    /// 账号门槛三档并存（开票平台族「需申请」、插入/报销族「公众号/服务号 ✔」、<c>scantitle</c>「仅认证」）；
    /// 与支付不耦合（仅消费 <c>mchid</c> 作配置）；16/17 页支持第三方平台代调用（权限集 <b>26</b>；<c>seturl</c>/<c>insert</c> 为 8、26），
    /// <b><c>scantitle</c> 独家不支持</b>；无频率数值；<b>错误码为族级共用表</b>（约 43 枚，多页重复列出同一张表）。
    /// </remarks>
    Invoice,

    /// <summary>
    /// 卡券 → 主体生命周期与投放 + 券码核销（<c>/card/*</c> 双接口同注册组共 14 端点：
    /// <c>IMpCardService</c> 11 端点建卡/查卡/改卡/库存/删除/二维码/落地页/两个卡面组件开关/测试白名单 +
    /// <c>IMpCardCodeService</c> 3 端点核销/查码状态/解密）。
    /// </summary>
    /// <remarks>
    /// 域级约束（<b>本批以本地 SKIT 源码为对齐基准，官方正文待逐页核验</b>）：
    /// 14 端点全为 POST + JSON、全部只消费应用级 <c>access_token</c>（Query 注入，MUD005 已知接受风险）；
    /// <b>前端取卡用的 <c>api_ticket</c> 已由 Abstractions 票据接口承载</b>，本域不引入第二套票据面；
    /// 与支付体系仅<b>透传</b> <c>merchant_id</c>、不调用支付接口；
    /// 建卡方向（<c>card</c> 包装 + <c>card_type</c> 判别 + 11 分支）与修改方向（<c>card_id</c> + 分支平级、
    /// 无 <c>advanced_info</c>）<b>不同构</b>，库存调整走独立端点（键名 <c>increase_stock_value</c>）。
    /// <b>未建模 10 族共 39 端点</b>（券码运维 5 / 会员卡与用户 7 / 礼品卡 11 / 付款礼品卡 4 /
    /// 兑换卡用户 1 / 特殊票券 3 / 子商户 4 / 门店小程序 2 / 卡券图文 1 / 协议查询 1（GET））
    /// 的取舍与理由由守卫 <c>MpCardContractGuards</c> CD8 留档。
    /// </remarks>
    Card,

    /// <summary>
    /// 微信门店旧版（POI）查询与删除（<c>/cgi-bin/poi/*</c> 3 端点：查询 / 列表 / 删除）。
    /// </summary>
    /// <remarks>
    /// <b>与 <c>Store</c> 域的关系（官方两代接口，并存勿合并）</b>：本域是旧版微信门店接口（POI），
    /// <c>Store</c> 是新版小程序店铺 API（<c>/wxa/*</c>）；门店 ID 体系不同。新建 / 更新官方未开放
    /// HTTP 新建入口（走后台）。落位由守卫 <c>MpPoiContractGuards</c> 锁定。
    /// </remarks>
    Poi,

    /// <summary>
    /// 语义理解（智能对话旧接口，<c>/semantic/semproxy/search</c> 1 端点）。
    /// </summary>
    /// <remarks>
    /// <b>停维警示</b>：官方长期未迭代，新项目应改用微信智能对话平台；SDK 按官方原样承载，
    /// details 以原始 JSON 透出（裁决见 <see cref="DataModels.Semantic.MpSemanticResult"/>）。
    /// 落位由守卫 <c>MpSemanticContractGuards</c> 锁定。
    /// </remarks>
    Semantic,
}
