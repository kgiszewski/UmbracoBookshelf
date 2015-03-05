module.exports = function(grunt) {
  require('load-grunt-tasks')(grunt);
  var path = require('path')

  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    pkgMeta: grunt.file.readJSON('config/meta.json'),
    dest: grunt.option('target') || 'dist',
    basePath: path.join('<%= dest %>', 'App_Plugins', '<%= pkgMeta.name %>'),

    watch: {
      options: {
        spawn: false,
        atBegin: true
      },

      html: {
        files: ['src/**/*.html'],
        tasks: ['copy:app_plugins']
      },

      dll: {
        files: ['src/bin/**/*.dll'],
        tasks: ['copy:dll']
      }
    },

    copy: {
      app_plugins: {
        cwd: 'src/App_Plugins/UmbracoBookshelf',
        src: ['**'],
        dest: '<%= basePath %>',
        expand: true
      },
      dll: {
        cwd: 'src/bin/Release/',
        src: 'UmbracoBookshelf.dll',
        dest: '<%= dest %>/bin/',
        expand: true
      },
      nuget: {
        files: [
          {
            cwd: '<%= dest %>/App_Plugins',
            src: ['**/*', '!bin', '!bin/*'],
            dest: 'tmp/nuget/content',
            expand: true
          },
          {
            cwd: '<%= dest %>/bin',
            src: ['*.dll'],
            dest: 'tmp/nuget/lib/net40',
            expand: true
          }
        ]
      },
      umbraco: {
        cwd: '<%= dest %>',
        src: '**/*',
        dest: 'tmp/umbraco',
        expand: true
      }
    },

    nugetpack: {
    	dist: {
    		src: 'tmp/nuget/package.nuspec',
    		dest: 'pkg'
    	}
    },

    template: {
    	'nuspec': {
			'options': {
    			'data': { 
            name: '<%= pkgMeta.name %>',
    				version: '<%= pkgMeta.version %>',
            url: '<%= pkgMeta.url %>',
            license: '<%= pkgMeta.license %>',
            licenseUrl: '<%= pkgMeta.licenseUrl %>',
            author: '<%= pkgMeta.author %>',
            authorUrl: '<%= pkgMeta.authorUrl %>',

    				files: [{ path: 'tmp/nuget/**', target: 'content/App_Plugins/UmbracoBookshelf'}]
	    		}
    		},
    		'files': { 
    			'tmp/nuget/package.nuspec': ['config/package.nuspec']
    		}
    	}
    },

    umbracoPackage: {
      options: {
        name: "<%= pkgMeta.name %>",
        version: '<%= pkgMeta.version %>',
        url: '<%= pkgMeta.url %>',
        license: '<%= pkgMeta.license %>',
        licenseUrl: '<%= pkgMeta.licenseUrl %>',
        author: '<%= pkgMeta.author %>',
        authorUrl: '<%= pkgMeta.authorUrl %>',
        manifest: 'config/package.xml',
        readme: 'config/readme.txt',
        sourceDir: 'tmp/umbraco',
        outputDir: 'pkg',
      }
    },

    clean: {
      build: '<%= grunt.config("basePath").substring(0, 4) == "dist" ? "dist/**/*" : "null" %>',
      tmp: ['tmp']
    },

    assemblyinfo: {
      options: {
        files: ['src/UmbracoBookshelf.csproj'],
        filename: 'AssemblyInfo.cs',
        info: {
          version: '<%= (pkgMeta.version.indexOf("-") ? pkgMeta.version.substring(0, pkgMeta.version.indexOf("-")) : pkgMeta.version) %>', 
          fileVersion: '<%= pkgMeta.version %>'
        }
      }
    },

    touch: {
      webconfig: {
        src: ['<%= grunt.option("target") %>\\Web.config']
      }
    },

    jshint: {
      options: {
        jshintrc: '.jshintrc'
      },
      src: {
        src: ['app/**/*.js', 'lib/**/*.js']
      }
    },

    msbuild: {
      options: {
        stdout: true,
        verbosity: 'quiet',
        maxCpuCount: 4,
		version: 4.0,
        buildParameters: {
          WarningLevel: 2,
          NoWarn: 1607
        }
      },
      dist: {
        src: ['src/UmbracoBookshelf.csproj'],
        options: {
          projectConfiguration: 'Release',
          targets: ['Clean', 'Rebuild'],
        }
      }
    }

  });

  grunt.registerTask('default', ['clean', 'assemblyinfo', 'msbuild:dist', 'copy:dll', 'copy:app_plugins']);

  grunt.registerTask('nuget',   ['clean:tmp', 'default', 'copy:nuget', 'template:nuspec', 'nugetpack', 'clean:tmp']);
  grunt.registerTask('umbraco', ['clean:tmp', 'default', 'copy:umbraco', 'umbracoPackage', 'clean:tmp']);
  grunt.registerTask('package', ['clean:tmp', 'default', 'copy:nuget', 'template:nuspec', 'nugetpack', 'copy:umbraco', 'umbracoPackage', 'clean:tmp']);
};