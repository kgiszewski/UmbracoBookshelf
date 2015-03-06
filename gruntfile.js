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
	  
	  less: {
        files: ['src/Assets/less/*.less'],
        tasks: ['less:dist']
      },

      app_plugins: {
        files: ['src/**/**'],
        tasks: ['copy:app_plugins', 'copy:initialContent']
      },

	  initialContent: {
        files: ['src/InitialContent/*'],
        tasks: ['copy:app_plugins', 'copy:initialContent']
      },

      dll: {
        files: ['src/**/*.cs'],
        tasks: ['msbuild:dist', 'copy:dll']
      }
    },
	
	less: {
      dist: {
        options: {
          paths: ["src/assets/less"],
        },
        files: {
          '<%= basePath %>/css/application.css': 'src/assets/less/application.less',
        }
      }
    },

    copy: {
      app_plugins: {
        cwd: 'src/App_Plugins/UmbracoBookshelf',
        src: ['**', '!css', '!css/*'],
        dest: '<%= basePath %>',
        expand: true
      },
      dll: {
        cwd: 'src/bin/debug/',
        src: 'UmbracoBookshelf.dll',
        dest: '<%= dest %>/bin/',
        expand: true
      },
	  actionsDll: {
        cwd: 'lib/',
        src: 'PackageActionsContrib.dll',
        dest: '<%= dest %>/bin/',
        expand: true
      },
	  initialContent: {
        cwd: 'src/InitialContent/UmbracoBookshelf/',
        src: ['**'],
        dest: '<%= dest %>/UmbracoBookshelf/',
        expand: true
      },
      nuget: {
        files: [
          {
            cwd: '<%= dest %>/App_Plugins',
            src: ['**/*', '!bin', '!bin/*'],
            dest: 'tmp/nuget/content/App_Plugins',
            expand: true
          },
		  {
            cwd: '<%= dest %>/UmbracoBookshelf/',
            src: ['**/*'],
            dest: 'tmp/nuget/content/UmbracoBookshelf',
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
					files: [{ path: 'tmp/nuget/content/App_Plugins', target: 'content/App_Plugins'}]
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
          projectConfiguration: 'Debug',
          targets: ['Clean', 'Rebuild'],
        }
      }
    }

  });

  grunt.registerTask('default', ['clean', 'less', 'assemblyinfo', 'msbuild:dist', 'copy:initialContent', 'copy:dll', 'copy:actionsDll', 'copy:app_plugins']);
  grunt.registerTask('nuget',   ['clean:tmp', 'default', 'copy:nuget', 'template:nuspec', 'nugetpack']);
  grunt.registerTask('umbraco', ['clean:tmp', 'default', 'copy:umbraco', 'umbracoPackage']);
  grunt.registerTask('package', ['clean:tmp', 'default', 'copy:nuget', 'template:nuspec', 'nugetpack', 'copy:umbraco', 'umbracoPackage', 'clean:tmp']);
};