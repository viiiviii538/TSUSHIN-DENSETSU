// lib/main.dart
import 'dart:async';
import 'dart:math';
import 'package:flutter/material.dart';

void main() => runApp(TsushinDensetsuApp());

class TsushinDensetsuApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: '通信伝説 - 簡易版',
      theme: ThemeData(
        useMaterial3: true,
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.blue),
      ),
      home: HomePage(),
    );
  }
}

class HomePage extends StatefulWidget {
  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage>
    with SingleTickerProviderStateMixin {
  late final TabController _tabController;
  String _speedResult = '未測定';
  String _securityResult = '未診断';
  String _topoResult = '未取得';
  bool _busy = false;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
  }

  Future<void> _simulateSpeedTest() async {
    if (_busy) return;
    setState(() {
      _busy = true;
      _speedResult = '測定中...';
    });
    await Future.delayed(Duration(milliseconds: 800));
    final rnd = Random();
    final down = (50 + rnd.nextDouble() * 450).toStringAsFixed(1); // Mbps
    final up = (5 + rnd.nextDouble() * 95).toStringAsFixed(1); // Mbps
    final ping = (5 + rnd.nextDouble() * 120).toStringAsFixed(0); // ms
    setState(() {
      _speedResult = '下り ${down} Mbps\n上り ${up} Mbps\nPING ${ping} ms';
      _busy = false;
    });
  }

  Future<void> _simulateSecurityScan() async {
    if (_busy) return;
    setState(() {
      _busy = true;
      _securityResult = '診断中...';
    });
    await Future.delayed(Duration(seconds: 1));
    final issues = ['RDP開放', '古いTLS', '公開FTP', '特記事項なし'];
    final rnd = Random();
    final picked = List.generate(2, (_) => issues[rnd.nextInt(issues.length)]);
    setState(() {
      _securityResult = '検出: ${picked.where((e) => e != '特記事項なし').join(', ')}';
      if (_securityResult == '検出: ') _securityResult = '検出なし';
      _busy = false;
    });
  }

  Future<void> _simulateTopologyScan() async {
    if (_busy) return;
    setState(() {
      _busy = true;
      _topoResult = '取得中...';
    });
    await Future.delayed(Duration(milliseconds: 900));
    setState(() {
      _topoResult =
          '機器数: 3\n- Router (Vendor: Mikrotik)\n- Server (Vendor: Dell)\n- Camera (Vendor: Hikvision)';
      _busy = false;
    });
  }

  Widget _buildCard(String title, String body, VoidCallback onPressed) {
    return Card(
      margin: EdgeInsets.symmetric(vertical: 12, horizontal: 16),
      child: Padding(
        padding: EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              title,
              style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
            ),
            SizedBox(height: 8),
            Text(body),
            SizedBox(height: 12),
            ElevatedButton(onPressed: onPressed, child: Text('実行')),
          ],
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('通信伝説（簡易）'),
        bottom: TabBar(
          controller: _tabController,
          tabs: [
            Tab(text: '速度測定'),
            Tab(text: 'セキュリティ診断'),
            Tab(text: '機器トポロジ'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: [
          // 速度測定タブ
          SingleChildScrollView(
            child: Column(
              children: [
                _buildCard('ネット速度を測定', _speedResult, _simulateSpeedTest),
                Padding(
                  padding: EdgeInsets.all(12),
                  child: Text('※ 現在は疑似測定です。後で実ネットワーク実装に差し替えます。'),
                ),
              ],
            ),
          ),
          // セキュリティ診断タブ
          SingleChildScrollView(
            child: Column(
              children: [
                _buildCard(
                  '簡易セキュリティ診断',
                  _securityResult,
                  _simulateSecurityScan,
                ),
                Padding(
                  padding: EdgeInsets.all(12),
                  child: Text('※ 脆弱性判定は将来的に詳細スキャンへ接続可能。'),
                ),
              ],
            ),
          ),
          // トポロジタブ
          SingleChildScrollView(
            child: Column(
              children: [
                _buildCard(
                  'LAN 機器をスキャンして表示',
                  _topoResult,
                  _simulateTopologyScan,
                ),
                Padding(
                  padding: EdgeInsets.all(12),
                  child: Text('※ 将来的に nmap / ARP スキャンと図表示に繋げます。'),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
