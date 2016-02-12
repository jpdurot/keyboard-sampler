'use strict';

var path = require('path');
var gulp = require('gulp');
var conf = require('../gulpfile.config');

var packageConfig = require('../package.json');

var $ = require('gulp-load-plugins')({
  pattern: ['gulp-*', 'main-bower-files', 'uglify-save-license', 'del']
});

function replaceConstant(string, replacement) {
  // Make sure we replace only the string located inside markers
  var constantRegExp = new RegExp('(// replace:constant[\\s\\S]*?)' + string + '([\\s\\S]*?endreplace)', 'gm');
  return $.replace(constantRegExp, '$1' + replacement + '$2')
}

function buildSources(target) {
  return function() {
    var htmlFilter = $.filter('*.html', {restore: true});
    var jsFilter = $.filter('**/*.js', {restore: true});
    var cssFilter = $.filter('**/*.css', {restore: true});
    var assets;
    var baseUri = '';
    if(target === 'prod') {
      baseUri = conf.baseUris.prod;
      conf.paths.dist = conf.paths.hybridDist;
    } else if(target === 'local') {
      baseUri = conf.baseUris.local;
    }

    return gulp.src(path.join(conf.paths.tmp, 'index.html'))
      .pipe($.replace(/<html/g, '<html ng-strict-di'))
      .pipe(assets = $.useref.assets())
      .pipe($.if('**/app*.js', replaceConstant('debug: true', 'debug: false')))
      .pipe($.if('**/app*.js', replaceConstant('version: \'dev\'', 'version: \'' + packageConfig.version + '\'')))
      .pipe($.if('**/app*.js', replaceConstant('baseUri: \'\'', 'baseUri: \'' + baseUri + '\'')))
      .pipe($.rev())
      .pipe(jsFilter)
      .pipe($.ngAnnotate())
      .pipe($.uglify({preserveComments: $.uglifySaveLicense})).on('error', conf.errorHandler('Uglify'))
      .pipe(jsFilter.restore)
      .pipe(cssFilter)
      .pipe($.minifyCss({ processImport: false }))
      .pipe(cssFilter.restore)
      .pipe(assets.restore())
      .pipe($.useref())
      .pipe($.revReplace())
      .pipe(htmlFilter)
      .pipe($.minifyHtml({
        empty: true,
        spare: true,
        quotes: true,
        conditionals: true
      }))
      .pipe(htmlFilter.restore)
      .pipe(gulp.dest(path.join(conf.paths.dist, '/')))
      .pipe($.size({title: path.join(conf.paths.dist, '/'), showFiles: true}));
  }
}

function addBaseUri(target) {

  return function() {

    function replacePath(string, replacement) {
      // Make sure we replace only the string located inside markers
      var constantRegExp = new RegExp('(<\!\-\- file \-\->[\\s\\S]*?)' + string + '([\\s\\S]*?endfile \-\->)', 'gm');
      console.log(string, replacement,constantRegExp);
      return $.replace(constantRegExp, '$1' + replacement + '$2')
    }

    var baseUri = '';
    if(target === 'prod') {
      baseUri = conf.baseUris.prod;
      conf.paths.dist = conf.paths.hybridDist;
    } else if(target === 'local') {
      baseUri = conf.baseUris.local;
    }

    return gulp.src(path.join(conf.paths.src, 'index.html'))
      .pipe($.replace('<script src="socket/hubs"></script>', '<script src="'+baseUri+'/socket/hubs"></script>'))
      .pipe(gulp.dest(conf.paths.tmp));
  }
}

gulp.task('baseuri:prod', addBaseUri('prod'));
gulp.task('baseuri:local', addBaseUri('local'));

gulp.task('build:sources', ['inject'], buildSources());
gulp.task('build:sources:prod', ['baseuri:prod', 'inject'], buildSources('prod'));
gulp.task('build:sources:local', ['baseuri:local', 'inject'], buildSources('local'));

// Only applies for fonts from bower dependencies
// Custom fonts are handled by the "other" task
gulp.task('fonts', function() {
  return gulp.src($.mainBowerFiles())
    .pipe($.filter('**/*.{eot,svg,ttf,woff,woff2}'))
    .pipe($.flatten())
    .pipe(gulp.dest(path.join(conf.paths.tmp, '/fonts/')));
});

gulp.task('other', ['fonts'], function() {
  var fileFilter = $.filter(function(file) {
    return file.stat.isFile();
  });

  return gulp.src([
      path.join(conf.paths.src, '/**/*'),
      path.join(conf.paths.tmp, '/**/*.{eot,svg,ttf,woff,woff2}'),
      path.join('!' + conf.paths.src, '/**/*.{html,css,js,ts,scss}'),
      path.join('!' + conf.paths.bower, '/**/*'),
      path.join('!' + conf.paths.src, '/translations/*'),
      path.join('!' + conf.paths.src, '/images/*')
    ])
    .pipe(fileFilter)
    .pipe(gulp.dest(path.join(conf.paths.dist, '/')));
});

gulp.task('build', ['build:sources', 'other', 'images']);
gulp.task('build:hybrid', ['build:sources:prod', 'other', 'images']);
gulp.task('build:local', ['build:sources:local', 'other', 'images']);

gulp.task('clean', function() {
  return $.del([path.join(conf.paths.hybridDist, '/'), path.join(conf.paths.tmp, '/')]);
});
