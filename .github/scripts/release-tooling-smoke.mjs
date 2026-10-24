import assert from 'node:assert/strict';
import fs from 'node:fs';
import path from 'node:path';
import { createRequire, syncBuiltinESMExports } from 'node:module';
import { pathToFileURL } from 'node:url';
import net from 'node:net';
import tls from 'node:tls';
import http from 'node:http';
import https from 'node:https';
import childProcess from 'node:child_process';

const project = path.resolve(process.argv[2] || '.');
const policy = path.resolve(process.argv[3] || project);
const require = createRequire(path.join(project, 'package.json'));
const attempts = [];
const deny = kind => () => { attempts.push(kind); throw new Error(`Hermetic consumer check blocked ${kind}`); };
net.Socket.prototype.connect = deny('network');
tls.connect = deny('TLS'); http.request = deny('HTTP'); https.request = deny('HTTPS');
globalThis.fetch = deny('fetch');
for (const key of ['spawn', 'spawnSync', 'exec', 'execSync', 'execFile', 'execFileSync', 'fork']) childProcess[key] = deny('child process ' + key);
syncBuiltinESMExports();
assert.equal(process.versions.node, '24.22.1');
const version = name => JSON.parse(fs.readFileSync(path.join(project, 'node_modules', name, 'package.json'), 'utf8')).version;
assert.equal(version('handlebars'), '4.7.10');
assert.equal(version('minimist'), '1.2.8');
assert.equal(version('npm'), '11.21.1');
assert.equal(version('npm/node_modules/brace-expansion'), '5.0.12');
assert.equal(version('npm/node_modules/undici'), '6.29.0'); // Explicit residual; bundled overrides do not replace it.
const h = require('handlebars').create();
assert.equal(h.compile('{{#each values}}{{@index}}={{this}};{{/each}}')({values: ['a', '<b>']}), '0=a;1=&lt;b&gt;;');
h.registerPartial('row', '[{{name}}]');
assert.equal(h.compile('{{#each values}}{{> row}}{{/each}}')({values: [{name: 'one'}, {name: 'two'}]}), '[one][two]');
const ast = h.parse('{{#if ok}}safe{{/if}}'); ast.body[0].program.blockParams = {length: '0'};
assert.throws(() => h.compile(ast)({ok: true}));
assert.equal(h.compile('{{lookup subject "constructor"}}')({subject: Function.prototype}, {allowProtoMethodsByDefault: true}), '');
const minimist = require('minimist');
assert.equal(minimist(['--value', 'fixture']).value, 'fixture');
minimist(['--__proto__.fixturePolluted', 'true']); assert.equal({}.fixturePolluted, undefined);
assert.deepEqual(require('brace-expansion')('asset-{one,two}.meta'), ['asset-one.meta', 'asset-two.meta']);
const imported = [];
for (const name of ['@semantic-release/git', '@semantic-release/github', '@semantic-release/npm', 'semantic-release-gitmoji']) {
  const loaded = await import(pathToFileURL(require.resolve(name)).href);
  assert.ok(Object.keys(loaded).length > 0, name); imported.push(name);
}
// Use the internal pure data/template class, never semantic-release or exported release hooks.
const ReleaseNotes = require(path.join(path.dirname(require.resolve('semantic-release-gitmoji')), 'lib/release-notes.js'));
const releaseTypes = require(path.join(path.dirname(require.resolve('semantic-release-gitmoji')), 'lib/assets/release-types.json'));
const releaseSchema = {...Object.fromEntries(releaseTypes.map(type => [type, []])), major: ['\u{1f4a5}'], minor: ['\u2728'], patch: ['\u{1f41b}']};
for (const [emoji, expected] of [['\u{1f4a5}', 'major'], ['\u2728', 'minor'], ['\u{1f41b}', 'patch']]) {
  const title = emoji + ' preserve local template compatibility (#12)';
  const context = {options: {repositoryUrl: 'https://github.com/example/public-fixture.git'},
    commits: [{hash: 'a'.repeat(40), subject: title, message: title, body: ''}],
    lastRelease: {version: '0.1.0', gitTag: 'v0.1.0'}, nextRelease: {version: '0.1.1', gitTag: 'v0.1.1'}};
  const notes = new ReleaseNotes(context, {template: '{{nextRelease.version}} {{#each commits.bug}}{{subject}}{{/each}}', partials: {}});
  assert.equal(notes.getReleaseType(releaseSchema), expected);
  await notes.updateContext(context); assert.match(notes.toString(), /0\.1\.1/);
  if (expected === 'patch') assert.match(notes.toString(), /preserve local template compatibility/);
}
const semanticPath = require.resolve('semantic-release'); // Resolve/read only; do not import or invoke the release engine.
const semanticRequire = createRequire(semanticPath);
const githubRequire = createRequire(semanticRequire.resolve('@semantic-release/github'));
// The already-locked http-client 4 publishes an import-only entry point.
// Anchor resolution at its physical manifest; do not pretend it exports require().
const httpClientRequire = createRequire(path.join(project, 'node_modules/@actions/http-client/package.json'));
const callers = [[httpClientRequire, '6.29.1'], [githubRequire, '7.30.1']];
for (const [caller, expected] of callers) {
  const api = caller('undici'); assert.equal(caller('undici/package.json').version, expected);
  const agent = new api.MockAgent(); agent.disableNetConnect();
  const pool = agent.get('https://fixture.invalid');
  pool.intercept({path: '/ok', method: 'GET'}).reply(200, {ok: true}, {headers: {'content-type': 'application/json'}});
  assert.deepEqual(await (await api.fetch('https://fixture.invalid/ok', {dispatcher: agent})).json(), {ok: true});
  pool.intercept({path: '/encoded', method: 'GET'}).reply(200, 'x', {headers: {'content-encoding': Array(20).fill('gzip').join(',')}});
  await assert.rejects(async () => { const response = await api.fetch('https://fixture.invalid/encoded', {dispatcher: agent}); await response.text(); });
  await agent.close();
}
const releaseDirectory = path.dirname(semanticPath);
const configSource = fs.readFileSync(path.join(releaseDirectory, 'lib/get-config.js'), 'utf8');
const engineSource = fs.readFileSync(semanticPath, 'utf8');
assert.ok(engineSource.indexOf('if (!context.branch)') < engineSource.indexOf('await plugins.verifyConditions(context)'));
const yaml = require('js-yaml');
const releaseConfig = yaml.load(fs.readFileSync(path.join(policy, '.releaserc.yml'), 'utf8'));
const patterns = releaseConfig.branches ? releaseConfig.branches.map(b => typeof b === 'string' ? b : b.name) : ['+([0-9])?(.{+([0-9]),x}).x', 'master', 'main', 'next', 'next-major', 'beta', 'alpha'];
if (!releaseConfig.branches) for (const pattern of patterns) assert.ok(configSource.includes(`"${pattern}"`));
const forbiddenRefs = ['refs/heads/codex/security-release-tooling', 'refs/heads/codex-security-release-tooling', 'refs/heads/feature/codex-check', 'refs/tags/codex/security', 'refs/tags/v1.0.0-codex'];
for (const ref of forbiddenRefs.filter(r => r.startsWith('refs/heads/'))) assert.deepEqual(require('micromatch')([ref.slice(11)], patterns), []);
const guard = "github.event_name == 'push' && startsWith(github.ref, 'refs/heads/') && !contains(github.ref_name, 'codex')";
let guardedJobs = 0;
let manualOnlyWorkflows = 0;
for (const file of fs.readdirSync(path.join(policy, '.github/workflows'))) {
  if (!/\.ya?ml$/.test(file)) continue;
  const source = fs.readFileSync(path.join(policy, '.github/workflows', file), 'utf8');
  const workflow = yaml.load(source); // The existing consumer parser rejects duplicate keys.
  const events = typeof workflow.on === 'string' ? [workflow.on] : Array.isArray(workflow.on) ? workflow.on : Object.keys(workflow.on || {});
  if (events.length === 1 && events[0] === 'workflow_dispatch') {
    manualOnlyWorkflows++;
    continue;
  }
  for (const [name, job] of Object.entries(workflow.jobs)) {
    const operations = JSON.stringify(job.steps || []);
    if (/semantic-release|upload-release|action-gh-release|create-unitypackage|zip-release/.test(operations)) {
      assert.ok(job.if === '${{ ' + guard + ' }}' || job.if?.startsWith('${{ ' + guard + ' && needs.'), name);
      guardedJobs++;
    }
  }
  for (const line of source.split('\n')) {
    if (/\bnpm\s+(ci|install)\b/.test(line)) for (const flag of ['--ignore-scripts', '--no-audit', '--no-fund']) assert.ok(line.includes(flag), line);
    assert.doesNotMatch(line, /\bnpm\s+audit\b/);
  }
}
assert.ok(guardedJobs >= 1);
for (const ref of forbiddenRefs) {
  const refName = ref.replace(/^refs\/(heads|tags)\//, '');
  assert.equal(ref.startsWith('refs/heads/') && !refName.toLowerCase().includes('codex'), false);
}
assert.deepEqual(attempts, []);
console.log(JSON.stringify({node: process.version, imported, guardedJobs, manualOnlyWorkflows, releaseEngineInvocations: 0, releaseHookInvocations: 0, networkAttempts: 0, childProcessAttempts: 0, passed: true}));
